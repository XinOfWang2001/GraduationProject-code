using Infra.Data.DatabaseContext;
using Infra.Data.DataSeeder;
using Leap.ApplicationServices.DTO.Calculations;
using Leap.Domain.Domain.Calculations;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Test.Leap.Backend.Fixtures;

namespace Test.Leap.Backend.IntegrationTests
{
    public class CalculationControllerTests : IClassFixture<LeapDSAPIAppFactory>, IAsyncLifetime
    {
        private readonly IServiceScope _scope;
        private readonly LeapDSDBContext _leapDSDBContext;
        private readonly TestDataSeeder testDataSeeder;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// NOTE: Current columns returned are SA-W-1_d-voeg_mm & SA-W-2_d-voeg_mm
        /// </summary>
        /// <param name="leapDSAPIAppFactory"></param>
        public CalculationControllerTests(LeapDSAPIAppFactory leapDSAPIAppFactory)
        {
            _scope = leapDSAPIAppFactory.Services.CreateScope();
            _leapDSDBContext = _scope.ServiceProvider.GetRequiredService<LeapDSDBContext>();
            _httpClient = leapDSAPIAppFactory.CreateClient();
            testDataSeeder = new TestDataSeeder(_leapDSDBContext);
        }

        // NA-XX Test Succesfull creation
        [Fact]
        public async Task TestSuccessfullKPICreation()
        {
            // Arrange
            int expectedSteps1 = 2;
            int expectedSteps2 = 1;
            string calculatedStringKpi1 = "KT * 2";
            string calculatedStringKpi2 = "KT2 + 50";
            Guid WorkspaceGuid = new Guid("324d5a66-ae3c-43a3-9e39-01c37ba4600e");
            KPIDTO kpi1 = new KPIDTO() { CalculationString = "KT * 2", InputColumns = ["KT "], OutputColumn = "KT_TIMES2", OperationsList = ["KT", "*", "2"] };
            KPIDTO kpi2 = new KPIDTO() { CalculationString = "KT2 + 50", InputColumns = ["KT2"], OutputColumn = "KTplus50", OperationsList = ["KT2", "+", "50"] };
            CalculationStepDTO steps = new CalculationStepDTO()
            {
                Order = 1,
                CalculationType = CalculationType.KPI,
                Calculations = new[] { kpi1, kpi2 }
            };
            CalculationStepDTO stepAnother50 = new CalculationStepDTO()
            {
                Order = 2,
                CalculationType = CalculationType.KPI,
                Calculations = new[] { kpi2 }
            };

            IEnumerable<CalculationStepDTO> StepsDTO = [steps, stepAnother50];

            CalculationWriteDTO RequestDTO = new CalculationWriteDTO() { Steps = StepsDTO, WorkspaceGuid = WorkspaceGuid };

            // Act
            var creationResponse = await _httpClient.PutAsJsonAsync($"api/calculations/", RequestDTO);

            Assert.True(creationResponse.IsSuccessStatusCode);
            //// Assert
            var ValidationResponse = await _httpClient.GetFromJsonAsync<CalculationRequestDTO>($"api/calculations/{WorkspaceGuid}");

            Assert.NotNull(ValidationResponse);
            Assert.True(ValidationResponse.Steps.Any());
            Assert.Equal(expectedSteps1, ValidationResponse.Steps.ElementAt(0).Calculations.Count());
            Assert.Equal(expectedSteps2, ValidationResponse.Steps.ElementAt(1).Calculations.Count());

            KPIDTO KPI1Step1 = (KPIDTO)ValidationResponse.Steps.ElementAt(0).Calculations.ElementAt(0);
            KPIDTO KPI1Step2 = (KPIDTO)ValidationResponse.Steps.ElementAt(0).Calculations.ElementAt(1);
            KPIDTO KPI2Step1 = (KPIDTO)ValidationResponse.Steps.ElementAt(1).Calculations.ElementAt(0);

            Assert.Equal(calculatedStringKpi1, KPI1Step1.CalculationString);
            Assert.Equal(calculatedStringKpi2, KPI1Step2.CalculationString);
            Assert.Equal(calculatedStringKpi2, KPI2Step1.CalculationString);
        }

        // NA-XX Test Successfull update with empty values
        [Fact]
        public async Task TestSuccesfullUpdateWithNoSteps()
        {
            int expectedSteps = 0;
            Guid WorkspaceGuid = new Guid("62734358-a671-4e1b-976d-743be6c0fead");

            IEnumerable<CalculationStepDTO> StepsDTO = [];

            CalculationWriteDTO RequestDTO = new CalculationWriteDTO() { Steps = StepsDTO, WorkspaceGuid = WorkspaceGuid };

            // Act
            var preview = await _httpClient.GetFromJsonAsync<CalculationRequestDTO>($"api/calculations/{WorkspaceGuid}");
            Assert.Equal(2, preview.Steps.Count());
            var creationResponse = await _httpClient.PutAsJsonAsync($"api/calculations/", RequestDTO);

            //// Assert
            Assert.True(creationResponse.IsSuccessStatusCode);
            var ValidationResponse = await _httpClient.GetFromJsonAsync<CalculationRequestDTO>($"api/calculations/{WorkspaceGuid}");
            Assert.False(ValidationResponse.Steps.Any());
        }

        // NA-XX Test Successfull update on existing calculations.
        [Fact]
        public async Task TestOverwriteFunction()
        {
            Guid WorkspaceGuid = new("c0e10697-0efb-4242-9d9f-dd2e26e359f0");
            string CalculationString = "KT + 3";
            IEnumerable<string> OperationList = ["KT", "+", "3"];
            string InputString = "KT";
            string OutputString = "KT";

            var preview = await _httpClient.GetFromJsonAsync<CalculationRequestDTO>($"api/calculations/{WorkspaceGuid}");
            Assert.Equal(2, preview.Steps.Count());
            Assert.Equal(2, preview.Steps.ElementAt(0).Calculations.Count());
            Assert.Equal(1, preview.Steps.ElementAt(1).Calculations.Count());

            KPIDTO KPI1Step1 = (KPIDTO)preview.Steps.ElementAt(0).Calculations.ElementAt(0);
            KPIDTO KPI1Step2 = (KPIDTO)preview.Steps.ElementAt(0).Calculations.ElementAt(1);
            KPIDTO KPI2Step1 = (KPIDTO)preview.Steps.ElementAt(1).Calculations.ElementAt(0);
            // Preview test
            Assert.Equal(CalculationString, KPI1Step1.CalculationString);
            Assert.Equal(CalculationString, KPI1Step2.CalculationString);
            Assert.Equal(CalculationString, KPI2Step1.CalculationString);

            Assert.Equal(OperationList, KPI1Step1.OperationsList);
            Assert.Equal(OperationList, KPI1Step2.OperationsList);
            Assert.Equal(OperationList, KPI2Step1.OperationsList);

            Assert.Equal([InputString], KPI1Step1.InputColumns);
            Assert.Equal([InputString], KPI1Step2.InputColumns);
            Assert.Equal([InputString], KPI2Step1.InputColumns);

            Assert.Equal(OutputString, KPI1Step1.OutputColumn);
            Assert.Equal(OutputString, KPI1Step2.OutputColumn);
            Assert.Equal(OutputString, KPI2Step1.OutputColumn);

            KPIDTO kpi1 = new KPIDTO() { CalculationString = "KT * 2", InputColumns = ["KT"], OutputColumn = "KT_times2", OperationsList = ["KT", "*", "2"] };

            CalculationStepDTO steps = new CalculationStepDTO()
            {
                Order = 1,
                CalculationType = CalculationType.KPI,
                Calculations = new[] { kpi1 }
            };

            IEnumerable<CalculationStepDTO> StepsDTO = [steps];

            CalculationWriteDTO RequestDTO = new CalculationWriteDTO() { Steps = StepsDTO, WorkspaceGuid = WorkspaceGuid };
            // Update
            var creationResponse = await _httpClient.PutAsJsonAsync($"api/calculations/", RequestDTO);

            // Assert
            var ValidationResponse = await _httpClient.GetFromJsonAsync<CalculationRequestDTO>($"api/calculations/{WorkspaceGuid}");

            Assert.NotNull(ValidationResponse);
            KPIDTO result = (KPIDTO)ValidationResponse.Steps.ElementAt(0).Calculations.ElementAt(0);

            Assert.True(ValidationResponse.Steps.Any());
            Assert.Single(ValidationResponse.Steps.ElementAt(0).Calculations);
            Assert.Equal("KT1 * 2", result.CalculationString);
            Assert.Equal(["KT", "*", "2"], result.OperationsList);
            Assert.Equal("KT_times2", result.OutputColumn);
            Assert.Equal(["KT"], KPI1Step2.InputColumns);
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
}
