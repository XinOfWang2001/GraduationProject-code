using Leap.ApplicationServices.DTO;
using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.DTO.DataResult;
using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.DTO.Workspace;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.Domain.Domain.ModelConfig.Enums;
using LeapDataScienceTool.Common.Services;
using LeapDataScienceTool.PageManagers;
using Moq;
using MudBlazor;

namespace Test.Leap.UITest.UC2_ModelTrainingConfig
{
    public class TestWorkspaceManager
    {
        // Non-functional
        [Fact]
        public async Task TestIfChangesInDataSourceDeletesInvalidColumns()
        {
            // Functional requirement: Databron selectie, Model training
            // Acceptance criteria: Synchronization of feature and target columns by changes in data source configuration.
            // Testcase: A change in observations, should delete datacolumns in Feature and target attributes.
            // Expected result: Columns not present in datacolumns should be removed.

            // Services
            Mock<IWorkspaceService> mockWorkspaceService = new Mock<IWorkspaceService>();
            Mock<IPreviewDataService> mockDataProxyService = new Mock<IPreviewDataService>();
            Mock<ISnackbar> mockPopup = new Mock<ISnackbar>();
            ResponseService responseService = new(mockPopup.Object);
            WorkspaceManager workspaceManager = new WorkspaceManager(mockWorkspaceService.Object, mockDataProxyService.Object, responseService);
            // Current collections
            IEnumerable<SensorDTO> CurrentSensors = [
                new () { Id = 1, Name = "SA-1" },
                new() { Id =12, Name = "SA-3" },
                new() { Id = 13, Name = "SA-4" },
                new() { Id = 14, Name = "SA-8" }
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
            IEnumerable<DataColumnDTO> CurrentLoadedColumns = [
                new() { Id = 1, ColumnName = "SA-1-DZ", DataType = "f64" },
                new() { Id = 3, ColumnName = "SA-3-DY", DataType = "f64" },
                new() { Id = 4, ColumnName = "SA-3-DZ", DataType = "f64" },
                new() { Id = 5, ColumnName = "SA-4-DZ", DataType = "f64" },
                new() { Id = 6, ColumnName = "SA-8-DZ", DataType = "f64" },
                ];
            // DTO
            DataExtractConfigDTO extractConfigDTO = new DataExtractConfigDTO()
            {
                AmountOfData = -1,
                SensorsSelected = CurrentSensors,
                ValueTypesSelected = CurrentVT
            };
            ModelConfigDTO modelConfigDTO = new()
            {
                ParentWorkspaceGuid = new Guid("8b90bcc5-bf6f-4ff1-b796-6cba0f51cf18"),
                Targets = CurrentTargets,
                Features = CurrentFeatureColumns,
                DataSplitRatio = 0.6f,
                ModelType = ModelType.FORECASTING,
                DateTimeLevel = DateTimeLevel.STANDARD,
                ModelAlgorithm = ModelAlgorithm.LINEAR_REGRESSION,
            };
            WorkspaceConfigDTO workspaceConfigDTO = new WorkspaceConfigDTO()
            {
                WorkspaceGuid = new Guid("8b90bcc5-bf6f-4ff1-b796-6cba0f51cf18"),
                WorkspaceName = "Test",
                DataSourceConfig = extractConfigDTO,
                ModelConfigDTO = modelConfigDTO
            };
            // Incoming data sources
            IEnumerable<DataColumnDTO> IncommingColumns = [
                new() { Id = 1, ColumnName = "SA-1-DZ", DataType = "f64"},
                new() { Id = 2, ColumnName = "SA-2-Temp", DataType = "f64" },
                new() { Id = 10, ColumnName = "SA-2-Length", DataType = "f64" },
                new() { Id = 11, ColumnName = "SA-9-DZ", DataType = "f64" },
                new() { Id = 11, ColumnName = "SA-9-DY", DataType = "f64" },
                ];

            IEnumerable<SensorDTO> Sensors = [
                new () { Id = 1, Name = "SA-1" },
                new () { Id = 2, Name = "SA-2" },
                new () { Id = 3, Name = "SA-9" }
                ];
            IEnumerable<ValueTypeDTO> UpdateVT = [
                new() { Id = 1, Name = "DZ" },
                new() { Id = 2, Name = "DY" },
                new() { Id = 3, Name = "Temp" },
                new() { Id = 4, Name = "Length" }];
            PreviewDataDTO preview = new PreviewDataDTO()
            {
                DataColumns = IncommingColumns,
                DataCount = 100,
            };
            mockDataProxyService.Setup(x => x.GetPreviewData(It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(preview);
            mockWorkspaceService.Setup(x => x.GetWorkspace(It.IsAny<Guid>())).ReturnsAsync(workspaceConfigDTO);
            extractConfigDTO.SensorsSelected = Sensors;
            extractConfigDTO.ValueTypesSelected = UpdateVT;
            await workspaceManager.LoadAllAssets(It.IsAny<Guid>());
            // Act 
            mockDataProxyService.Setup(x => x.GetPreviewData(It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(preview);
            await workspaceManager.UpdateDataSourceConfig(extractConfigDTO);

            // Assert
            ModelConfigDTO modelconfig = workspaceManager.GetModelConfig();
            Assert.NotNull(modelconfig);
            Assert.Equal("SA-1-DZ", modelconfig.Features.ElementAt(0).ColumnName);
            Assert.Empty(modelconfig.Targets);
        }
    }
}
