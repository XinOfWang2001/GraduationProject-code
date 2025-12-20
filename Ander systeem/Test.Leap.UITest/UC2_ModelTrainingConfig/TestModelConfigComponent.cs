using Bunit;
using Leap.ApplicationServices.DTO;
using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.DTO.Workspace;
using LeapDataScienceTool.Components.ModelingProcess;
using LeapDataScienceTool.PageManagers;
using LeapDataScienceTool.ProgramSetup;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor;
using Test.Leap.UITest.ExtensionMethods;

namespace Test.Leap.UITest.UC2_ModelTrainingConfig
{
    public class TestModelConfigComponent : TestContext
    {
        private readonly Mock<IDialogService> _dialogService;

        public TestModelConfigComponent()
        {
            _dialogService = new Mock<IDialogService>();
        }
        [Fact]
        public void TestInitialLoad()
        {
            Services.RegisterProxyServices();
            Services.RegisterUIComponents();

            Mock<IWorkspaceManager> Manager = new Mock<IWorkspaceManager>();
            Manager.Setup(x => x.GetModelConfig()).Returns((ModelConfigDTO?)null);
            Manager.Setup(x => x.GetWorkspaceConfigDTO()).Returns(new WorkspaceConfigDTO());
            Services.AddSingleton(Manager.Object);

            var Component = RenderComponent<ModelingComponent>();

            Assert.Equal(string.Empty, Component.Instance.ModelConfigWarning);
        }

        // Code: BZ-15
        [Fact]
        public void TestIfInconsistentFeatureAndTargetColumnsRaisesErrorOnScreen()
        {
            IEnumerable<DataColumnDTO> IncommingColumns = [
                new() { Id = 1, ColumnName = "SA-1-DZ", DataType = "f64"},
                new() { Id = 2, ColumnName = "SA-2-Temp", DataType = "f64" },
                new() { Id = 10, ColumnName = "SA-2-Length", DataType = "f64" },
                new() { Id = 11, ColumnName = "SA-9-DZ", DataType = "f64" },
                new() { Id = 11, ColumnName = "SA-9-DY", DataType = "f64" },
                ];

            IEnumerable<DataColumnDTO> WrongFeatures = [
                new () { Id = 15, ColumnName ="Irrelevant", DataType = "f64" }
                ];
            IEnumerable<DataColumnDTO> WrongTarget = [
                new () { Id = 16, ColumnName ="Irrelevant2", DataType = "f64" }
                ];
            Services.RegisterProxyServices();
            Services.RegisterUIComponents();

            Mock<IWorkspaceManager> Manager = new Mock<IWorkspaceManager>();
            Manager.Setup(x => x.GetModelConfig()).Returns(new ModelConfigDTO()
            {
                ParentWorkspaceGuid = Guid.NewGuid(),
                Features = WrongFeatures,
                Targets = WrongTarget
            });
            Manager.Setup(x => x.GetColumns()).Returns(IncommingColumns);
            Services.AddSingleton(Manager.Object);

            var Component = RenderComponent<ModelingComponent>();

            Assert.Equal("Gekozen feature variabelen zijn invalide. Wijzig deze gegevens graag.", Component.Instance.ModelConfigWarning);
        }

        // Code: BZ-15
        // Use case: UC-2 Model training configuration
        // Functional requirement: Succesfull configuration.
        // Testcase: Check if successfull request handles ok.
        [Fact]
        public async Task TestSuccesfullRequestModelConfig()
        {
            Mock<IDialogReference> _dialogResult = new Mock<IDialogReference>();
            IEnumerable<DataColumnDTO> IncommingColumns = [
                new() { Id = 1, ColumnName = "SA-1-DZ", DataType = "f64"},
                new() { Id = 2, ColumnName = "SA-2-Temp", DataType = "f64" },
                new() { Id = 10, ColumnName = "SA-2-Length", DataType = "f64" },
                new() { Id = 11, ColumnName = "SA-9-DZ", DataType = "f64" },
                new() { Id = 11, ColumnName = "SA-9-DY", DataType = "f64" },
                ];

            IEnumerable<DataColumnDTO> RightFeatures = [
                new () { Id = 15, ColumnName ="SA-1-DZ", DataType = "f64" }
                ];
            IEnumerable<DataColumnDTO> RightTargets = [
                new () { Id = 16, ColumnName ="SA-2-Temp", DataType = "f64" }
                ];
            ModelConfigDTO configDTO = new ModelConfigDTO()
            {
                ParentWorkspaceGuid = Guid.NewGuid(),
                Features = RightFeatures,
                Targets = RightTargets
            };
            Services.RegisterProxyServices();
            Services.RegisterUIComponents();

            Mock<IWorkspaceManager> Manager = new Mock<IWorkspaceManager>();
            Manager.Setup(x => x.GetModelConfig()).Returns(configDTO);
            Manager.Setup(x => x.GetColumns()).Returns(IncommingColumns);
            Services.AddSingleton(Manager.Object);
            _dialogResult.Setup(x => x.GetReturnValueAsync<ModelConfigDTO>()).ReturnsAsync(configDTO);
            _dialogService.Setup(x => x.ShowAsync<ModelConfigDialog>(It.IsAny<string>(), It.IsAny<DialogParameters>(), It.IsAny<DialogOptions>())).ReturnsAsync(_dialogResult.Object);

            Services.AddSingleton(_dialogService.Object);

            var Component = RenderComponent<ModelingComponent>();
            // Act
            await Component.InvokeAsync(() => Component.Instance.OpenForm());

            Assert.NotNull(Component.Instance.modelConfig);
        }
    }
}
