using Infra.Data.DatabaseContext;
using Infra.Data.DataSeeder;
using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.Domain.Domain.ModelConfig.Enums;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Test.Leap.Backend.Fixtures;

namespace Test.Leap.Backend.IntegrationTests
{
    public class ModelConfigControllerTest : IClassFixture<LeapDSAPIAppFactory>, IAsyncLifetime
    {
        private readonly IServiceScope _scope;
        private readonly LeapDSDBContext _leapDSDBContext;
        private readonly TestDataSeeder testDataSeeder;
        private readonly HttpClient _httpClient;

        public ModelConfigControllerTest(LeapDSAPIAppFactory leapDSAPIAppFactory)
        {
            _scope = leapDSAPIAppFactory.Services.CreateScope();
            _leapDSDBContext = _scope.ServiceProvider.GetRequiredService<LeapDSDBContext>();
            _httpClient = leapDSAPIAppFactory.CreateClient();
            testDataSeeder = new TestDataSeeder(_leapDSDBContext);
        }

        [Fact]
        public async Task TestGetOneEndpointModelConfig()
        {
            Guid existingGuid = new("5b3ad50a-a9b5-4972-8e8c-30d948e213b9");
            var result = await _httpClient.GetAsync($"/api/model/{existingGuid}");

            ModelConfigDTO response = await result.Content.ReadFromJsonAsync<ModelConfigDTO>();

            Assert.NotNull(response.AlgorithmParameterDTO);
            Assert.NotNull(response.Features);
            Assert.NotNull(response.Targets);
            Assert.Equal(existingGuid, response.ModelConfigGuid);
        }

        // A-24
        [Fact]
        public async Task TestCreationSuccesful()
        {
            string endpoint = $"/api/model";
            LinearRegressionDTO linearRegressionDTO = new LinearRegressionDTO()
            {
                NJobs = 5,
            };
            ModelConfigDTO postEntity = new ModelConfigDTO()
            {
                ModelConfigGuid = new Guid("a550ab50-6c3a-4b44-85cd-2a6de62daae6"),
                ModelName = "Config Nr. 3",
                ModelAlgorithm = ModelAlgorithm.LINEAR_REGRESSION,
                ModelType = ModelType.FORECASTING,
                DateTimeLevel = DateTimeLevel.STANDARD,
                DataSplitRatio = 0.9f,
                ForecastingDate = new DateTime(2025, 4, 3),
                ParentWorkspaceGuid = new Guid("41f75185-cd16-4bd4-8f8b-717d2b0385ea"),
                AlgorithmParameterDTO = linearRegressionDTO,
                Features = [new() { ColumnName = "sw_3", DataType = "f64" }],
                Targets = [new() { ColumnName = "sw_5", DataType = "f64" }],
            };

            var result = await _httpClient.PostAsJsonAsync(endpoint, postEntity);

            Assert.True(result.IsSuccessStatusCode);

            var get_result = await _httpClient.GetAsync($"/api/model/{postEntity.ModelConfigGuid}");
            ModelConfigDTO response = await get_result.Content.ReadFromJsonAsync<ModelConfigDTO>();

            Assert.NotNull(response);

            // Duplicate request should return 400 error.
            var resultDuplicate = await _httpClient.PostAsJsonAsync(endpoint, postEntity);
            Assert.False(resultDuplicate.IsSuccessStatusCode);
        }

        // A-31
        [Fact]
        public async Task TestCreationMissingTargets()
        {
            string endpoint = $"/api/model";
            LinearRegressionDTO linearRegressionDTO = new LinearRegressionDTO()
            {
                NJobs = 5,
            };
            ModelConfigDTO postEntity = new ModelConfigDTO()
            {
                ModelConfigGuid = new Guid("a550ab50-6c3a-4b44-85cd-2a6de62daae6"),
                ModelName = "Config Nr. 3",
                ModelAlgorithm = ModelAlgorithm.LINEAR_REGRESSION,
                ModelType = ModelType.FORECASTING,
                DateTimeLevel = DateTimeLevel.STANDARD,
                DataSplitRatio = 0.9f,
                ForecastingDate = new DateTime(2025, 4, 3),
                ParentWorkspaceGuid = new Guid("41f75185-cd16-4bd4-8f8b-717d2b0385ea"),
                AlgorithmParameterDTO = linearRegressionDTO,
                Features = [new() { ColumnName = "sw_3", DataType = "f64" }],
                Targets = [],
            };

            var result = await _httpClient.PostAsJsonAsync(endpoint, postEntity);

            Assert.False(result.IsSuccessStatusCode);
        }

        // A-32
        [Fact]
        public async Task TestUpdateRegularAttributeChange()
        {
            // Arrange
            SVMDTO algorithmDTO = new()
            {
                Kernel = "sigmoid"
            };
            ModelConfigDTO updateEntity = new()
            {
                ModelConfigGuid = new Guid("4b1085cc-e369-41ca-b658-8aab7b18ebc7"),
                ModelName = "Config Nr. 3-1",
                ModelAlgorithm = ModelAlgorithm.SVMREGRESSION,
                ModelType = ModelType.FORECASTING,
                DateTimeLevel = DateTimeLevel.STANDARD,
                DataSplitRatio = 0.9f,
                ForecastingDate = new DateTime(2025, 10, 10),
                ParentWorkspaceGuid = new Guid("9ef039ff-4e90-41dc-991a-08cd899bcc72"),
                AlgorithmParameterDTO = algorithmDTO,
                Features = [new() { ColumnName = "sw_4", DataType = "f64" }, new() { ColumnName = "sw_8", DataType = "f64" }],
                Targets = [new() { ColumnName = "sw_6", DataType = "f64" }],
            };
            string endpoint = $"/api/model/{updateEntity.ModelConfigGuid}";
            // Act
            var result = await _httpClient.PutAsJsonAsync(endpoint, updateEntity);
            Assert.True(result.IsSuccessStatusCode);

            // Assert
            var validatorResponse = await _httpClient.GetAsync(endpoint);
            ModelConfigDTO? modelConfigDTO = await validatorResponse.Content.ReadFromJsonAsync<ModelConfigDTO>();
            Assert.Equal(2, modelConfigDTO!.Features.Count());
            Assert.Single(modelConfigDTO!.Targets);
            Assert.Equal("sw_4", modelConfigDTO.Features.ElementAt(0).ColumnName);
            Assert.Equal("sw_8", modelConfigDTO.Features.ElementAt(1).ColumnName);
            Assert.Equal("sw_6", modelConfigDTO.Targets.ElementAt(0).ColumnName);

            Assert.Equal(ModelAlgorithm.SVMREGRESSION, modelConfigDTO.ModelAlgorithm);
            Assert.Equal(0.9f, modelConfigDTO.DataSplitRatio);
        }

        public Task DisposeAsync()
        {
            testDataSeeder.DeleteData();
            return Task.CompletedTask;
        }

        public Task InitializeAsync()
        {
            return testDataSeeder.SeedModelConfigs();
        }
    }
}
