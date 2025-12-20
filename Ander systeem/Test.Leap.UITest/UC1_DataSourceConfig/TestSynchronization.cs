using Bunit;
using Leap.ApplicationServices.DTO;
using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.DTO.DataProcessDTO;
using Leap.ApplicationServices.DTO.DataResult;
using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.DTO.Workspace;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.Domain.Domain.ModelConfig.Enums;
using LeapDataScienceTool.Common.Services;
using LeapDataScienceTool.Components.DataSourceProcess;
using LeapDataScienceTool.Components.ModelingProcess;
using LeapDataScienceTool.PageManagers;
using LeapDataScienceTool.Pages;
using LeapDataScienceTool.ProgramSetup;
using LeapDataScienceTool.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor;
using MudBlazor.Services;

namespace Test.Leap.UITest.UC1_DataSourceConfig
{
    public class TestSynchronization : TestContext
    {
        private readonly Mock<IWorkspaceService> mockWorkspaceService;
        private readonly Mock<IPreviewDataService> mockDataProxyService;
        private readonly Mock<ICalculationService> mockCalculationService;

        // Mudservices
        private readonly Mock<IMudDialogInstance> _dialogInstance;
        private readonly Mock<IDialogService> _dialogService;
        private readonly Mock<IWorkspaceService> _workspaceService;
        private readonly Mock<IPreviewDataService> _dataProxyService;
        private readonly Mock<IModelService> _modelProxyService;
        private readonly Mock<MudLocalizer> _mudInternalLocalizer;
        private readonly Mock<ISnackbar> _popComponent;
        private readonly ResponseService _responseService;

        public TestSynchronization()
        {
            mockWorkspaceService = new Mock<IWorkspaceService>();
            mockDataProxyService = new Mock<IPreviewDataService>();
            _dialogInstance = new Mock<IMudDialogInstance>();
            _dialogService = new Mock<IDialogService>();
            _workspaceService = new Mock<IWorkspaceService>();
            _dataProxyService = new Mock<IPreviewDataService>();
            _modelProxyService = new Mock<IModelService>();
            mockCalculationService = new Mock<ICalculationService>();
            _popComponent = new Mock<ISnackbar>();
            _responseService = new ResponseService(_popComponent.Object);
            _mudInternalLocalizer = new Mock<MudLocalizer>();
        }

        private void LoadServices()
        {
            Services.AddSingleton(mockCalculationService.Object);
            Services.AddSingleton(_dialogInstance.Object);
            Services.AddSingleton(_dialogService.Object);
            Services.AddSingleton(_dataProxyService.Object);
            Services.AddSingleton(_workspaceService.Object);
            Services.AddSingleton(_modelProxyService.Object);
            Services.AddSingleton(_popComponent.Object);
            Services.AddSingleton(_mudInternalLocalizer.Object);
            Services.AddSingleton<ResponseService>();
            JSInterop.SetupVoid("mudDragAndDrop.initDropZone", _ => true);
            JSInterop.SetupVoid("mudKeyInterceptor.connect", _ => true);
            Services.RegisterRuntimeClasses();
            Services.AddMudServices();
            Services.AddScoped<IWorkspaceManager, WorkspaceManager>();
            Services.AddScoped<IAlgorithmComponentBuilder, AlgorithmBuilder>();
        }

        private static WorkspaceConfigDTO GetBaseDTO()
        {
            // Current collections
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
            // DTO
            ProjectSourceDTO project = new ProjectSourceDTO() { Guid = Guid.NewGuid(), HumanReadableName = "Test bron", Name = "test_bron", Id = -1 };
            DataSourceDTO dataSource = new DataSourceDTO() { Name = "test_source" };
            DataExtractConfigDTO extractConfigDTO = new DataExtractConfigDTO()
            {
                AmountOfData = -1,
                SensorsSelected = CurrentSensors,
                ValueTypesSelected = CurrentVT,
                ProjectDTO = project,
                DataSource = dataSource,
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
            return workspaceConfigDTO;
        }


        /// <summary>
        /// Code: BZ-12
        /// Use case: UC-1, UC-2
        /// Requirement: Synchronization of datasource changes.
        /// Testcase: Change sensor selcection from [SA-1, SA-3, SA-4, SA-8] to [SA-1, SA-2, SA-9] And VT [DZ, DY] to [DZ, DY, Temp, Length]
        /// Expected result: 
        /// Component should raise an error stating: "Stel nieuwe waarden in" 
        /// </summary>
        [Fact]
        public async Task TestIfChangesInDataSourceShowsWarningsInModelingComponent()
        {
            // Arrange
            Mock<IDialogReference> _dialogResult = new Mock<IDialogReference>();
            LoadServices();

            // Return values
            IEnumerable<SensorDTO> UpdateSensors = [
                new () { Id = 1, Name = "SA-1" },
                new() { Id =12, Name = "SA-3" },
                new() { Id = 13, Name = "SA-4" },
                new() { Id = 14, Name = "SA-8" }
                ];
            IEnumerable<ValueTypeDTO> UpdateVT = [
                new() { Id = 1, Name = "DZ" }];

            IEnumerable<DataColumnDTO> IncommingColumns = [
                new() { Id = 1, ColumnName = "SA-1-DZ", DataType = "f64"},
                new() { Id = 2, ColumnName = "SA-2-Temp", DataType = "f64" },
                new() { Id = 10, ColumnName = "SA-2-Length", DataType = "f64" },
                new() { Id = 11, ColumnName = "SA-9-DZ", DataType = "f64" },
                new() { Id = 11, ColumnName = "SA-9-DY", DataType = "f64" },
                ];
            PreviewDataDTO newPreviewData = new PreviewDataDTO()
            {
                DataColumns = IncommingColumns,
                DataCount = 100,
            };

            // Current previewdata
            IEnumerable<DataColumnDTO> CurrentLoadedColumns = [
                new() { Id = 1, ColumnName = "SA-1-DZ", DataType = "f64" },
                new() { Id = 3, ColumnName = "SA-3-DY", DataType = "f64" },
                new() { Id = 4, ColumnName = "SA-3-DZ", DataType = "f64" },
                new() { Id = 5, ColumnName = "SA-4-DZ", DataType = "f64" },
                new() { Id = 6, ColumnName = "SA-8-DZ", DataType = "f64" },
                ];
            PreviewDataDTO currentPreviewData = new PreviewDataDTO()
            {
                DataColumns = CurrentLoadedColumns,
                DataCount = 100,
            };
            // DTO
            WorkspaceConfigDTO workspaceConfigDTO = GetBaseDTO();

            mockWorkspaceService.Setup(param => param.GetWorkspace(It.IsAny<Guid>())).ReturnsAsync(workspaceConfigDTO);
            mockDataProxyService.Setup(param => param.GetPreviewData(It.IsAny<Guid>(), false)).ReturnsAsync(currentPreviewData);
            // WorkspaceManager
            var WorkspaceManager = new WorkspaceManager(mockWorkspaceService.Object, mockDataProxyService.Object, _responseService);
            Services.AddSingleton<IWorkspaceManager>(WorkspaceManager);

            // Components
            var WorkspaceEditComponent = RenderComponent<WorkspaceEditPage>();
            var DataSourceEditComponent = WorkspaceEditComponent.FindComponent<DataSourceComponent>();
            var ModelingComponent = WorkspaceEditComponent.FindComponent<ModelingComponent>();
            // Initial check before.
            Assert.NotNull(DataSourceEditComponent);
            Assert.Equal(string.Empty, ModelingComponent.Instance.ModelConfigWarning);
            // Act
            DataExtractConfigDTO updatedExtractConfig = workspaceConfigDTO.DataSourceConfig;
            updatedExtractConfig.SensorsSelected = UpdateSensors;
            updatedExtractConfig.ValueTypesSelected = UpdateVT;

            mockDataProxyService.Setup(param => param.GetPreviewData(It.IsAny<Guid>(), false)).ReturnsAsync(newPreviewData);
            _dialogResult.Setup(x => x.GetReturnValueAsync<DataExtractConfigDTO>()).ReturnsAsync(updatedExtractConfig);
            _dialogService.Setup(x => x.ShowAsync<DataSourceDialog>(It.IsAny<string>(), It.IsAny<DialogParameters>(), It.IsAny<DialogOptions>())).ReturnsAsync(_dialogResult.Object);

            await DataSourceEditComponent.InvokeAsync(() => DataSourceEditComponent.Instance.ChooseVariables());

            // Assert
            Assert.Equal("Stel nieuwe waarden in", ModelingComponent.Instance.ModelConfigWarning);
        }

        /// <summary>
        /// Code BZ-13
        /// Use case: UC-1, UC-2
        /// Requirement: Synchronization of datasource changes.
        /// Testcase: Change no sensor selcection from [SA-1, SA-3, SA-4, SA-8] And Change VT [DZ, DY] to [DY]
        /// Expected result: 
        /// Component should raise an error stating: "Stel nieuwe waarden in" 
        /// </summary>
        [Fact]
        public async Task TestIfChangesInVTChoicesShowsErrorMessage()
        {
            // Arrange
            Mock<IDialogReference> _dialogResult = new Mock<IDialogReference>();
            LoadServices();

            // Return values
            IEnumerable<SensorDTO> UpdateSensors = [
                new () { Id = 1, Name = "SA-1" },
                new() { Id =12, Name = "SA-3" },
                new() { Id = 13, Name = "SA-4" },
                new() { Id = 14, Name = "SA-8" },
                ];
            IEnumerable<ValueTypeDTO> UpdateVT = [
                new() { Id = 1, Name = "DZ" },
                new() { Id = 2, Name = "DY" }];
            IEnumerable<DataColumnDTO> IncommingColumns = [
                new() { Id = 3, ColumnName = "SA-3-DY", DataType = "f64" },
                ];
            PreviewDataDTO newPreviewData = new PreviewDataDTO()
            {
                DataColumns = IncommingColumns,
                DataCount = 100,
            };

            // Current previewdata
            IEnumerable<DataColumnDTO> CurrentLoadedColumns = [
                new() { Id = 1, ColumnName = "SA-1-DZ", DataType = "f64" },
                new() { Id = 3, ColumnName = "SA-3-DY", DataType = "f64" },
                new() { Id = 4, ColumnName = "SA-3-DZ", DataType = "f64" },
                new() { Id = 5, ColumnName = "SA-4-DZ", DataType = "f64" },
                new() { Id = 6, ColumnName = "SA-8-DZ", DataType = "f64" },
                ];
            PreviewDataDTO currentPreviewData = new PreviewDataDTO()
            {
                DataColumns = CurrentLoadedColumns,
                DataCount = 100,
            };
            // DTO
            WorkspaceConfigDTO workspaceConfigDTO = GetBaseDTO();

            mockWorkspaceService.Setup(param => param.GetWorkspace(It.IsAny<Guid>())).ReturnsAsync(workspaceConfigDTO);
            mockDataProxyService.Setup(param => param.GetPreviewData(It.IsAny<Guid>(), false)).ReturnsAsync(currentPreviewData);
            // WorkspaceManager
            var WorkspaceManager = new WorkspaceManager(mockWorkspaceService.Object, mockDataProxyService.Object, _responseService);
            Services.AddSingleton<IWorkspaceManager>(WorkspaceManager);

            // Components
            var WorkspaceEditComponent = RenderComponent<WorkspaceEditPage>();
            var DataSourceEditComponent = WorkspaceEditComponent.FindComponent<DataSourceComponent>();
            var ModelingComponent = WorkspaceEditComponent.FindComponent<ModelingComponent>();
            // Initial check before.
            Assert.NotNull(DataSourceEditComponent);
            Assert.Equal(string.Empty, ModelingComponent.Instance.ModelConfigWarning);
            // Act
            DataExtractConfigDTO updatedExtractConfig = workspaceConfigDTO.DataSourceConfig;
            updatedExtractConfig.SensorsSelected = UpdateSensors;
            updatedExtractConfig.ValueTypesSelected = UpdateVT;

            mockDataProxyService.Setup(param => param.GetPreviewData(It.IsAny<Guid>(), false)).ReturnsAsync(newPreviewData);
            _dialogResult.Setup(x => x.GetReturnValueAsync<DataExtractConfigDTO>()).ReturnsAsync(updatedExtractConfig);
            _dialogService.Setup(x => x.ShowAsync<DataSourceDialog>(It.IsAny<string>(), It.IsAny<DialogParameters>(), It.IsAny<DialogOptions>())).ReturnsAsync(_dialogResult.Object);

            await DataSourceEditComponent.InvokeAsync(() => DataSourceEditComponent.Instance.ChooseVariables());

            // Assert
            Assert.Equal("Stel nieuwe waarden in", ModelingComponent.Instance.ModelConfigWarning);
        }

        /// <summary>
        /// Code: BZ-14
        /// Use case: UC-1, UC-2
        /// Requirement: Synchronization of datasource changes.
        /// Testcase: Change no sensor selcection from [SA-1, SA-3, SA-4, SA-8] And Change VT [DZ, DY] to [DY]
        /// Expected result: 
        /// No Error is shown on screen.
        /// </summary>
        /// 
        [Fact]
        public async Task TestChangeRedundantSensorShouldNotShowError()
        {
            // Arrange
            Mock<IDialogReference> _dialogResult = new Mock<IDialogReference>();
            LoadServices();

            // Return values
            IEnumerable<SensorDTO> UpdateSensors = [
                new () { Id = 1, Name = "SA-1" },
                new() { Id = 12, Name = "SA-3" },
                new() { Id = 13, Name = "SA-4" },
                new() { Id = 14, Name = "SA-8" }
                ];
            IEnumerable<ValueTypeDTO> UpdateVT = [
                new() { Id = 1, Name = "DZ" },
                new() { Id = 2, Name = "DY" },];
            IEnumerable<DataColumnDTO> IncommingColumns = [
                new() { Id = 1, ColumnName = "SA-1-DZ", DataType = "f64" },
                new() { Id = 3, ColumnName = "SA-3-DY", DataType = "f64" },
                new() { Id = 4, ColumnName = "SA-3-DZ", DataType = "f64" },
                new() { Id = 5, ColumnName = "SA-4-DZ", DataType = "f64" },
                new() { Id = 6, ColumnName = "SA-8-DZ", DataType = "f64" },
                ];
            PreviewDataDTO newPreviewData = new PreviewDataDTO()
            {
                DataColumns = IncommingColumns,
                DataCount = 100,
            };

            // Current previewdata
            IEnumerable<DataColumnDTO> CurrentLoadedColumns = [
                new() { Id = 1, ColumnName = "SA-1-DZ", DataType = "f64" },
                new() { Id = 3, ColumnName = "SA-3-DY", DataType = "f64" },
                new() { Id = 4, ColumnName = "SA-3-DZ", DataType = "f64" },
                new() { Id = 5, ColumnName = "SA-4-DZ", DataType = "f64" },
                new() { Id = 6, ColumnName = "SA-8-DZ", DataType = "f64" },
                ];
            PreviewDataDTO currentPreviewData = new PreviewDataDTO()
            {
                DataColumns = CurrentLoadedColumns,
                DataCount = 100,
            };
            // DTO
            WorkspaceConfigDTO workspaceConfigDTO = GetBaseDTO();

            mockWorkspaceService.Setup(param => param.GetWorkspace(It.IsAny<Guid>())).ReturnsAsync(workspaceConfigDTO);
            mockDataProxyService.Setup(param => param.GetPreviewData(It.IsAny<Guid>(), false)).ReturnsAsync(currentPreviewData);
            // WorkspaceManager
            var WorkspaceManager = new WorkspaceManager(mockWorkspaceService.Object, mockDataProxyService.Object, _responseService);
            Services.AddSingleton<IWorkspaceManager>(WorkspaceManager);

            // Components
            var WorkspaceEditComponent = RenderComponent<WorkspaceEditPage>();
            var DataSourceEditComponent = WorkspaceEditComponent.FindComponent<DataSourceComponent>();
            var ModelingComponent = WorkspaceEditComponent.FindComponent<ModelingComponent>();
            // Initial check before.
            Assert.NotNull(DataSourceEditComponent);
            Assert.Equal(string.Empty, ModelingComponent.Instance.ModelConfigWarning);
            // Act
            DataExtractConfigDTO updatedExtractConfig = workspaceConfigDTO.DataSourceConfig;
            updatedExtractConfig.SensorsSelected = UpdateSensors;
            updatedExtractConfig.ValueTypesSelected = UpdateVT;

            mockDataProxyService.Setup(param => param.GetPreviewData(It.IsAny<Guid>(), false)).ReturnsAsync(newPreviewData);
            _dialogResult.Setup(x => x.GetReturnValueAsync<DataExtractConfigDTO>()).ReturnsAsync(updatedExtractConfig);
            _dialogService.Setup(x => x.ShowAsync<DataSourceDialog>(It.IsAny<string>(), It.IsAny<DialogParameters>(), It.IsAny<DialogOptions>())).ReturnsAsync(_dialogResult.Object);

            await DataSourceEditComponent.InvokeAsync(() => DataSourceEditComponent.Instance.ChooseVariables());

            // Assert
            Assert.Equal(string.Empty, ModelingComponent.Instance.ModelConfigWarning);
        }
    }
}
