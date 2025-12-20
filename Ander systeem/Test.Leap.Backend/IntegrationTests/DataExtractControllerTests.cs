using Infra.Data.DatabaseContext;
using Infra.Data.DataSeeder;
using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.DTO.DataProcessDTO;
using Leap.ApplicationServices.DTO.External_Services;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Test.Leap.Backend.Fixtures;

namespace Test.Leap.Backend.IntegrationTests;

public class DataExtractControllerTests : IClassFixture<LeapDSAPIAppFactory>, IAsyncLifetime
{
    private readonly IServiceScope _scope;
    private readonly LeapDSDBContext _leapDSDBContext;
    private readonly TestDataSeeder testDataSeeder;
    private readonly HttpClient _httpClient;

    // LET-OP: De data is afhankelijk van de seeding data, binnen de migratiescript.
    public DataExtractControllerTests(LeapDSAPIAppFactory leapDSAPIAppFactory)
    {
        _scope = leapDSAPIAppFactory.Services.CreateScope();
        _leapDSDBContext = _scope.ServiceProvider.GetRequiredService<LeapDSDBContext>();
        _httpClient = leapDSAPIAppFactory.CreateClient();
        testDataSeeder = new TestDataSeeder(_leapDSDBContext);
    }

    [Fact]
    public async Task TestDatabaseContext()
    {
        var response = await _httpClient.GetAsync("api/Workspace");
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
    }

    // Code A-12
    [Fact]
    public async Task TestIfIncompleteDTOReturnsError()
    {
        // Arrange
        DataExtractConfigDTO dto = new DataExtractConfigDTO()
        {
            WorkspaceId = new Guid("a86ff674-ae5a-472a-9479-aaacb5f5ce9e"),
            StartDate = new DateTime(2024, 11, 11),
            EndDate = new DateTime(2024, 12, 1),
            DataSource = new DataSourceDTO() { DataSourceId = 2 },
            SensorsSelected = new List<SensorDTO> { new SensorDTO { Id = 1, Name = "C-1" } },
            ValueTypesSelected = new List<ValueTypeDTO> { new ValueTypeDTO { Id = 1, Name = "Temp" } },
            TimeLevelDTO = new TimeLevelDTO() { TimelevelId = 1, TimelevelName = "Seconds", TimelevelRange = 1000000 }
            // No Project
        };
        // Act
        var response = await _httpClient.PostAsJsonAsync("api/dataextract", dto);
        DataExtractConfigDTO responseBody = await response.Content.ReadFromJsonAsync<DataExtractConfigDTO>();
        Assert.Equal(400, responseBody.StatusCode);
    }

    // Code A-11
    [Fact]
    public async Task TestIfPostRequestSucceeds()
    {
        // Arrange
        DataExtractConfigDTO dto = new DataExtractConfigDTO()
        {
            WorkspaceId = new Guid("a86ff674-ae5a-472a-9479-aaacb5f5ce9e"),
            StartDate = new DateTime(2024, 11, 11),
            EndDate = new DateTime(2024, 12, 1),
            DataSource = new DataSourceDTO() { DataSourceId = 2 },
            SensorsSelected = new List<SensorDTO> { new SensorDTO { Id = 1, Name = "C-1" } },
            ValueTypesSelected = new List<ValueTypeDTO> { new ValueTypeDTO { Id = 1, Name = "Temp" } },
            ProjectDTO = new ProjectSourceDTO() { Id = 1, HumanReadableName = "KT", Guid = new Guid("77d3c0ea-91b5-4e6f-9e1e-f2937edfd167"), Name = "KT" },
            TimeLevelDTO = new TimeLevelDTO() { TimelevelId = 1, TimelevelName = "Seconds", TimelevelRange = 1000000 }
        };
        // Act
        var response = await _httpClient.PostAsJsonAsync("api/dataextract", dto);

        DataExtractConfigDTO responseBody = await response.Content.ReadFromJsonAsync<DataExtractConfigDTO>();

        Assert.Equal("Succesvol opgeslagen", responseBody.Message);

        var getResponse = await _httpClient.GetFromJsonAsync<DataExtractConfigDTO>($"api/dataextract/{responseBody.ProcessId}");
        Assert.NotNull(getResponse);
        Assert.Single(getResponse.SensorsSelected);
        Assert.Single(getResponse.ValueTypesSelected);
    }

    // Code UC-13
    [Fact]
    public async Task TestIfDuplicatePostFails()
    {
        // Arrange
        DataExtractConfigDTO dto = new DataExtractConfigDTO()
        {
            WorkspaceId = new Guid("9ef039ff-4e90-41dc-991a-08cd899bcc72"),
            ProcessId = new Guid("07302e29-ba87-425f-a690-5a5eab7461ee"),
            StartDate = new DateTime(2024, 11, 11),
            EndDate = new DateTime(2024, 12, 1),
            DataSource = new DataSourceDTO() { DataSourceId = 2 },
            SensorsSelected = new List<SensorDTO> { new SensorDTO { Id = 1, Name = "C-1" } },
            ValueTypesSelected = new List<ValueTypeDTO> { new ValueTypeDTO { Id = 1, Name = "Temp" } },
            TimeLevelDTO = new TimeLevelDTO() { TimelevelId = 1, TimelevelName = "Seconds", TimelevelRange = 1000000 },
            ProjectDTO = new ProjectSourceDTO() { Id = 1, HumanReadableName = "KT", Guid = new Guid("77d3c0ea-91b5-4e6f-9e1e-f2937edfd167"), Name = "KT" },
        };
        // Act
        await _httpClient.PostAsJsonAsync("api/dataextract", dto);
        // The same request
        var response2 = await _httpClient.PostAsJsonAsync("api/dataextract", dto);

        DataExtractConfigDTO responseBody2 = await response2.Content.ReadFromJsonAsync<DataExtractConfigDTO>();
        Assert.Equal("DataExtracter entiteit bestaat al", responseBody2.Message);
    }

    // Code A-14
    [Fact]
    public async Task UpdateSuccesful()
    {
        // Arrange
        SensorDTO sensor = new SensorDTO() { Id = 1, Name = "C-1" };
        SensorDTO sensor2 = new SensorDTO() { Id = 2, Name = "C-2" };
        SensorDTO sensor3 = new SensorDTO() { Id = 3, Name = "C-3" };
        ValueTypeDTO valueType1 = new ValueTypeDTO { Id = 1, Name = "Temp" };
        ValueTypeDTO valueType2 = new ValueTypeDTO { Id = 2, Name = "Temp2" };
        ValueTypeDTO valueType3 = new ValueTypeDTO { Id = 3, Name = "Temp2" };
        Guid processId = new Guid("07302e29-ba87-425f-a690-5a5eab7461ee");
        DataExtractConfigDTO dto1 = new DataExtractConfigDTO()
        {
            WorkspaceId = new Guid("41f75185-cd16-4bd4-8f8b-717d2b0385ea"),
            ProcessId = processId,
            StartDate = new DateTime(2024, 11, 11),
            EndDate = new DateTime(2024, 12, 1),
            DataSource = new DataSourceDTO() { DataSourceId = 2 },
            SensorsSelected = new List<SensorDTO> { sensor, sensor2 },
            ValueTypesSelected = new List<ValueTypeDTO> { valueType1, valueType3 },
            ProjectDTO = new ProjectSourceDTO() { Id = 1, HumanReadableName = "KT", Guid = new Guid("77d3c0ea-91b5-4e6f-9e1e-f2937edfd167"), Name = "KT" },
            TimeLevelDTO = new TimeLevelDTO() { TimelevelId = 1, TimelevelName = "Seconds", TimelevelRange = 1000000 }
        };
        DataExtractConfigDTO updateDTO = new DataExtractConfigDTO()
        {
            WorkspaceId = new Guid("41f75185-cd16-4bd4-8f8b-717d2b0385ea"),
            ProcessId = processId,
            StartDate = new DateTime(2024, 8, 11),
            EndDate = new DateTime(2024, 12, 1),
            DataSource = new DataSourceDTO() { DataSourceId = 2 },
            SensorsSelected = new List<SensorDTO> { sensor, sensor3 },
            ValueTypesSelected = new List<ValueTypeDTO> { valueType2, valueType3 },
            ProjectDTO = new ProjectSourceDTO() { Id = 1, HumanReadableName = "KT", Guid = new Guid("77d3c0ea-91b5-4e6f-9e1e-f2937edfd167"), Name = "KT" },
            TimeLevelDTO = new TimeLevelDTO() { TimelevelId = 2, TimelevelName = "Seconds", TimelevelRange = 2000000 }
        };
        // Act
        var response = await _httpClient.PostAsJsonAsync("api/dataextract", dto1);
        DataExtractConfigDTO responseBody2 = await response.Content.ReadFromJsonAsync<DataExtractConfigDTO>();

        Assert.Equal("Succesvol opgeslagen", responseBody2.Message);
        Assert.Equal(1, responseBody2.ValueTypesSelected.ElementAt(0).Id);
        Assert.Equal(3, responseBody2.ValueTypesSelected.ElementAt(1).Id);
        Assert.Equal(1, responseBody2.SensorsSelected.ElementAt(0).Id);
        Assert.Equal(2, responseBody2.SensorsSelected.ElementAt(1).Id);

        updateDTO.ProcessId = responseBody2.ProcessId;
        var updateResponse = await _httpClient.PutAsJsonAsync($"api/dataextract/{responseBody2.ProcessId}", updateDTO);

        DataExtractConfigDTO updateResponseDTO = await updateResponse.Content.ReadFromJsonAsync<DataExtractConfigDTO>();
        Assert.Equal("Succesvol gewijzigd.", updateResponseDTO.Message);
        Assert.Equal(2, updateResponseDTO.ValueTypesSelected.ElementAt(0).Id);
        Assert.Equal(3, updateResponseDTO.ValueTypesSelected.ElementAt(1).Id);
        // Sensor validation
        Assert.Equal(1, updateResponseDTO.SensorsSelected.ElementAt(0).Id);
        Assert.Equal(3, updateResponseDTO.SensorsSelected.ElementAt(1).Id);
        // Timelevel validation
        Assert.Equal(2, updateResponseDTO.TimeLevelDTO.TimelevelId);
        Assert.Equal("Seconds", updateResponseDTO.TimeLevelDTO.TimelevelName);
        Assert.Equal(2000000, updateResponseDTO.TimeLevelDTO.TimelevelRange);
    }

    public Task InitializeAsync()
    {
        return testDataSeeder.SeedWorkshops();
    }

    public Task DisposeAsync()
    {
        testDataSeeder.DeleteData();
        return Task.CompletedTask;
    }
}
