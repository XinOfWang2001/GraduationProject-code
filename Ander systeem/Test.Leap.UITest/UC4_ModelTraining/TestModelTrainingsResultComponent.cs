using Bunit;
using Leap.ApplicationServices.DTO;
using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.DTO.DataProcessDTO;
using Leap.ApplicationServices.DTO.DataResult;
using Leap.ApplicationServices.DTO.External_Services;
using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.DTO.ModelingProcess;
using Leap.ApplicationServices.DTO.Workspace;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.Domain.Domain.ModelConfig.Enums;
using LeapDataScienceTool.API;
using LeapDataScienceTool.Components.ModelTrainingComponent;
using LeapDataScienceTool.PageManagers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Test.Leap.UITest.ExtensionMethods;

namespace Test.Leap.UITest.UC4_ModelTraining
{
    public class TestModelTrainingsResultComponent : TestContext
    {
        // BZ-27
        // Use cases: UC-2 Modeltraining, UC-4 Forecasting data genereren.
        // Testcase: Test if successfull returns a dataset, including the metrics MAPE and RSME From selected predicted values
        // Expected result: A Modelresult object not null, RSME has a value. Show message SuccesfullText = "Modeltraining is succesvol verlopen."
        [Fact]
        public void TestSuccesfullComponentLoading()
        {
            // Arrange
            IEnumerable<SensorDTO> CurrentSensors = [
                new () { Id = 1, Name = "SA-1" },
                new() { Id =12, Name = "SA-3" },
                new() { Id = 13, Name = "SA-4" },
                new() { Id = 14, Name = "SA-8" },
                new() { Id = 14, Name = "SW-10" }
                ];
            IEnumerable<ValueTypeDTO> CurrentVT = [
                new() { Id = 1, Name = "DZ" },
                new() { Id = 2, Name = "DY" }];
            IEnumerable<DataColumnDTO> CurrentFeatureColumns = [
                new() { Id = 1, ColumnName = "SA-1-DZ", DataType = "f64" },
                new() { Id = 3, ColumnName = "SA-3-DY", DataType = "f64" }];
            IEnumerable<DataColumnDTO> CurrentTargets = [
                new() { Id = 5, ColumnName = "SA-4-DZ", DataType = "f64" },
                new() { Id = 6, ColumnName = "SA-8-DZ", DataType = "f64" }
            ];
            Guid WorkspaceGuid = Guid.NewGuid();
            DataExtractConfigDTO configDTO = new()
            {
                WorkspaceId = WorkspaceGuid,
                SensorsSelected = CurrentSensors,
                ValueTypesSelected = CurrentVT,
                AmountOfData = 5,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(4),
                ProjectDTO = new ProjectSourceDTO() { Guid = Guid.NewGuid(), Name = "test_bron", HumanReadableName = "Test bron" },
                TimeLevelDTO = new TimeLevelDTO() { TimelevelId = 1, TimelevelName = "1 Second", TimelevelRange = 1000 },
            };
            WorkspaceConfigDTO workspace = new()
            {
                WorkspaceGuid = WorkspaceGuid,
                WorkspaceName = "Werkruimte"
            };
            ModelConfigDTO modelConfig = new()
            {
                ParentWorkspaceGuid = WorkspaceGuid,
                ModelType = ModelType.FORECASTING,
                ModelAlgorithm = ModelAlgorithm.LINEAR_REGRESSION,
                DateTimeLevel = DateTimeLevel.ONLY_DATES,
                AlgorithmParameterDTO = new LinearRegressionDTO(),
                Features = [new() { ColumnName = "SW-1", DataType = "f64" }],
                Targets = [new() { ColumnName = "SW-2", DataType = "f64" }],
                ModelConfigGuid = Guid.NewGuid(),
            };

            Mock<IServerAPI> MockServerAPI = new();
            Mock<IWorkspaceManager> manager = new();
            Mock<IWorkspaceService> service = new();
            Mock<IPreviewDataService> dataService = new();

            manager.Setup(m => m.GetDataExtractConfigDTO()).Returns(configDTO);
            manager.Setup(m => m.GetWorkspaceConfigDTO()).Returns(workspace);
            manager.Setup(m => m.GetModelConfig()).Returns(modelConfig);

            ModelResultDataDTO result = new()
            {
                DataSet = new DataSeries()
                {
                    Timestamps = [
                        new DateTime(2023, 11, 26, 12, 30, 00),
                        new DateTime(2024, 12, 3, 12, 30, 00),
                        new DateTime(2024, 12, 10, 12, 30, 00),
                        new DateTime(2024, 12, 17, 12, 30, 00),
                         new DateTime(2024, 12, 24, 12, 30, 00),
                    ],
                    ColumnNames = ["SW-2"],
                    Values = new Dictionary<string, IEnumerable<float>>()
                    {
                        {"SW-2", [11.4f, 12.5f, 12.6f, 13.2f, 13.4f] }
                    }
                },
                PredictionSet = new DataSeries()
                {
                    Timestamps = [
                        new DateTime(2024, 1, 1, 12, 30, 00),
                        new DateTime(2024, 1, 8, 12, 30, 00),
                        new DateTime(2024, 1, 15, 12, 30, 00),
                        new DateTime(2024, 1, 22, 12, 30, 00),
                         new DateTime(2024, 1, 29, 12, 30, 00),
                    ],
                    ColumnNames = ["SW-2_predicted"],
                    Values = new Dictionary<string, IEnumerable<float>>()
                    {
                        {"SW-2_predicted", [12.3f, 15.2f, 15.6f, 16.2f, 16.8f] }
                    }
                },
                MetricsKeyValue = new() {
                    { "MAPE", [ new() { Column = "SW-2",  Metric = "MAPE", Value = 0.01f }] },
                    { "RMSE", [ new() { Column = "SW-2", Metric = "RSME", Value = 1.2f }] }
                },
            };

            MockServerAPI.Setup(ms => ms.Post<ModelResultDataDTO>(It.IsAny<string>(), It.IsAny<ModelTrainingRequestDTO>())).ReturnsAsync(result);

            Services.AddSingleton(MockServerAPI.Object);

            Services.AddSingleton(service.Object);
            Services.AddSingleton(dataService.Object);
            Services.RegisterUIComponents();
            // To override WorkspaceManager implementation.
            Services.AddSingleton(manager.Object);
            // Act
            var ModelTrainingResultDialog = RenderComponent<ModelTrainingResultDialog>();
            var Component = ModelTrainingResultDialog.Instance;

            // Assert
            Assert.Equal(1, ModelTrainingResultDialog.RenderCount);
            Assert.NotNull(ModelTrainingResultDialog.Instance.Result);
            Assert.Equal("Modeltraining is succesvol verlopen.", ModelTrainingResultDialog.Instance.SuccesfullText);
        }

        // Code  BZ-28 N-BZ-39a
        // Use cases: UC-2 Modeltraining, UC-4 Forecasting data genereren.
        // Testcase: Test if failed training request shows error message
        // Expected result: ModelResult is null, Succesfull text = "Er was iets misgegaan met het modeltrainen. Neem contact op met de ontwikkelaars van deze tool en probeer het later nog eens."
        [Fact]
        public void TestFailedTrainingShouldShowErrorMessage()
        {
            // Arrange
            IEnumerable<SensorDTO> CurrentSensors = [
                new () { Id = 1, Name = "SA-1" },
                new() { Id =12, Name = "SA-3" },
                new() { Id = 13, Name = "SA-4" },
                new() { Id = 14, Name = "SA-8" },
                new() { Id = 14, Name = "SW-10" }
                ];
            IEnumerable<ValueTypeDTO> CurrentVT = [
                new() { Id = 1, Name = "DZ" },
                new() { Id = 2, Name = "DY" }];
            IEnumerable<DataColumnDTO> CurrentFeatureColumns = [
                new() { Id = 1, ColumnName = "SA-1-DZ", DataType = "f64" },
                new() { Id = 3, ColumnName = "SA-3-DY", DataType = "f64" }];
            IEnumerable<DataColumnDTO> CurrentTargets = [
                new() { Id = 5, ColumnName = "SA-4-DZ", DataType = "f64" },
                new() { Id = 6, ColumnName = "SA-8-DZ", DataType = "f64" }
            ];
            Guid WorkspaceGuid = Guid.NewGuid();
            DataExtractConfigDTO configDTO = new()
            {
                WorkspaceId = WorkspaceGuid,
                SensorsSelected = CurrentSensors,
                ValueTypesSelected = CurrentVT,
                AmountOfData = 5,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(4),
                ProjectDTO = new ProjectSourceDTO() { Guid = Guid.NewGuid(), Name = "test_bron", HumanReadableName = "Test bron" },
                TimeLevelDTO = new TimeLevelDTO() { TimelevelId = 1, TimelevelName = "1 Second", TimelevelRange = 1000 },
            };
            WorkspaceConfigDTO workspace = new()
            {
                WorkspaceGuid = WorkspaceGuid,
                WorkspaceName = "Werkruimte"
            };
            ModelConfigDTO modelConfig = new()
            {
                ParentWorkspaceGuid = WorkspaceGuid,
                ModelType = ModelType.FORECASTING,
                ModelAlgorithm = ModelAlgorithm.LINEAR_REGRESSION,
                DateTimeLevel = DateTimeLevel.ONLY_DATES,
                AlgorithmParameterDTO = new LinearRegressionDTO(),
                Features = [new() { ColumnName = "SW-1", DataType = "f64" }],
                Targets = [new() { ColumnName = "SW-2", DataType = "f64" }],
                ModelConfigGuid = Guid.NewGuid(),
            };

            Mock<IServerAPI> MockServerAPI = new();
            Mock<IWorkspaceManager> manager = new();
            Mock<IWorkspaceService> service = new();
            Mock<IPreviewDataService> dataService = new();

            manager.Setup(m => m.GetDataExtractConfigDTO()).Returns(configDTO);
            manager.Setup(m => m.GetWorkspaceConfigDTO()).Returns(workspace);
            manager.Setup(m => m.GetModelConfig()).Returns(modelConfig);

            ModelResultDataDTO result = new()
            {
                DataSet = new DataSeries()
                {
                    Timestamps = [
                        new DateTime(2023, 11, 26, 12, 30, 00),
                        new DateTime(2024, 12, 3, 12, 30, 00),
                        new DateTime(2024, 12, 10, 12, 30, 00),
                        new DateTime(2024, 12, 17, 12, 30, 00),
                         new DateTime(2024, 12, 24, 12, 30, 00),
                    ],
                    ColumnNames = ["SW-2"],
                    Values = new Dictionary<string, IEnumerable<float>>()
                    {
                        {"SW-2", [11.4f, 12.5f, 12.6f, 13.2f, 13.4f] }
                    }
                },
                PredictionSet = new DataSeries()
                {
                    Timestamps = [
                        new DateTime(2024, 1, 1, 12, 30, 00),
                        new DateTime(2024, 1, 8, 12, 30, 00),
                        new DateTime(2024, 1, 15, 12, 30, 00),
                        new DateTime(2024, 1, 22, 12, 30, 00),
                         new DateTime(2024, 1, 29, 12, 30, 00),
                    ],
                    ColumnNames = ["SW-2_predicted"],
                    Values = new Dictionary<string, IEnumerable<float>>()
                    {
                        {"SW-2_predicted", [12.3f, 15.2f, 15.6f, 16.2f, 16.8f] }
                    }
                },
                MetricsKeyValue = new() {
                    { "MAPE", [ new() { Column = "SW-2",  Metric = "MAPE", Value = 0.01f }] },
                    { "RMSE", [ new() { Column = "SW-2", Metric = "RSME", Value = 1.2f }] }
                },
            };

            MockServerAPI.Setup(ms => ms.Post<ModelResultDataDTO>(It.IsAny<string>(), It.IsAny<ModelTrainingRequestDTO>())).ReturnsAsync((ModelResultDataDTO?)null);

            Services.AddSingleton(MockServerAPI.Object);
            Services.AddSingleton(service.Object);
            Services.AddSingleton(dataService.Object);
            Services.RegisterUIComponents();
            // To override WorkspaceManager implementation.
            Services.AddSingleton(manager.Object);
            // Act
            var ModelTrainingResultDialog = RenderComponent<ModelTrainingResultDialog>();
            var Component = ModelTrainingResultDialog.Instance;
            // Assert
            Assert.Equal("Er was iets misgegaan met het modeltrainen. Neem contact op met de ontwikkelaars van deze tool en probeer het later nog eens.", Component.SuccesfullText);
            Assert.Null(Component.Result);
        }


        // Code BZ-29, N-BZ-39b
        // Use cases: UC-2 Timeseries forecasting, UC-4 Forecasting data genereren.
        // Testcase: Test if UI component returns error message if model configuration has not been completed.
        // Expected result: Show error message: "Databron configuratie of Modelconfiguratie moeten ingesteld worden."
        [Fact]
        public void TestFailedValidationCheckShouldRaiseError()
        {
            // Arrange
            IEnumerable<SensorDTO> CurrentSensors = [
                new () { Id = 1, Name = "SW-1" },
                new() { Id = 14, Name = "SW-2" }
                ];
            IEnumerable<ValueTypeDTO> CurrentVT = [
                new() { Id = 1, Name = "DZ" }
                ];
            IEnumerable<DataColumnDTO> CurrentFeatureColumns = [
                new() { Id = 1, ColumnName = "SW-1_DZ", DataType = "f64" },
                new() { Id = 3, ColumnName = "SW-2_DZ", DataType = "f64" }];
            IEnumerable<DataColumnDTO> CurrentTargets = [
                new() { Id = 6, ColumnName = "SW-2_DZ", DataType = "f64" }
            ];
            Guid WorkspaceGuid = Guid.NewGuid();
            DataExtractConfigDTO configDTO = new()
            {
                WorkspaceId = WorkspaceGuid,
                SensorsSelected = CurrentSensors,
                ValueTypesSelected = CurrentVT,
                AmountOfData = 5,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(4),
                ProjectDTO = new ProjectSourceDTO() { Guid = Guid.NewGuid(), Name = "test_bron", HumanReadableName = "Test bron" },
                TimeLevelDTO = new TimeLevelDTO() { TimelevelId = 1, TimelevelName = "1 Second", TimelevelRange = 1000 },
            };
            WorkspaceConfigDTO workspace = new()
            {
                WorkspaceGuid = WorkspaceGuid,
                WorkspaceName = "Werkruimte"
            };
            ModelConfigDTO modelConfig = new()
            {
                ParentWorkspaceGuid = WorkspaceGuid,
                ModelType = ModelType.FORECASTING,
                ModelAlgorithm = ModelAlgorithm.LINEAR_REGRESSION,
                DateTimeLevel = DateTimeLevel.ONLY_DATES,
                AlgorithmParameterDTO = new LinearRegressionDTO(),
                Features = CurrentFeatureColumns,
                Targets = CurrentTargets,
                ModelConfigGuid = Guid.NewGuid(),
            };

            Mock<IServerAPI> MockServerAPI = new();
            Mock<IWorkspaceManager> manager = new();
            Mock<IWorkspaceService> service = new();
            Mock<IPreviewDataService> dataService = new();

            manager.Setup(m => m.GetDataExtractConfigDTO()).Returns(configDTO);
            manager.Setup(m => m.GetWorkspaceConfigDTO()).Returns(workspace);
            manager.Setup(m => m.GetModelConfig()).Returns(modelConfig);

            ModelResultDataDTO result = new()
            {
                DataSet = new DataSeries()
                {
                    Timestamps = [
                        new DateTime(2023, 11, 26, 12, 30, 00),
                        new DateTime(2024, 12, 3, 12, 30, 00),
                        new DateTime(2024, 12, 10, 12, 30, 00),
                        new DateTime(2024, 12, 17, 12, 30, 00),
                         new DateTime(2024, 12, 24, 12, 30, 00),
                    ],
                    ColumnNames = ["SW-2"],
                    Values = new Dictionary<string, IEnumerable<float>>()
                    {
                        {"SW-2", [11.4f, 12.5f, 12.6f, 13.2f, 13.4f] }
                    }
                },
                PredictionSet = new DataSeries()
                {
                    Timestamps = [
                        new DateTime(2024, 1, 1, 12, 30, 00),
                        new DateTime(2024, 1, 8, 12, 30, 00),
                        new DateTime(2024, 1, 15, 12, 30, 00),
                        new DateTime(2024, 1, 22, 12, 30, 00),
                         new DateTime(2024, 1, 29, 12, 30, 00),
                    ],
                    ColumnNames = ["SW-2_predicted"],
                    Values = new Dictionary<string, IEnumerable<float>>()
                    {
                        {"SW-2_predicted", [12.3f, 15.2f, 15.6f, 16.2f, 16.8f] }
                    }
                },
                MetricsKeyValue = new() {
                    { "MAPE", [ new() { Column = "SW-2",  Metric = "MAPE", Value = 0.01f }] },
                    { "RMSE", [ new() { Column = "SW-2", Metric = "RSME", Value = 1.2f }] }
                },
            };

            MockServerAPI.Setup(ms => ms.Post<ModelResultDataDTO>(It.IsAny<string>(), It.IsAny<ModelTrainingRequestDTO>())).Throws(new Exception());

            Services.AddSingleton(MockServerAPI.Object);
            Services.AddSingleton(service.Object);
            Services.AddSingleton(dataService.Object);
            Services.RegisterUIComponents();
            // To override WorkspaceManager implementation.
            Services.AddSingleton(manager.Object);
            // Act
            var ModelTrainingResultDialog = RenderComponent<ModelTrainingResultDialog>();
            var Component = ModelTrainingResultDialog.Instance;
            // Assert
            Assert.Equal("Databron configuratie of Modelconfiguratie moeten nog ingesteld worden.", Component.SuccesfullText);
            Assert.Null(Component.Result);
        }
    }
}
