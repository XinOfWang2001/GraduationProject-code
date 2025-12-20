using Bunit;
using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.Domain.Domain.ModelConfig.Enums;
using LeapDataScienceTool.API;
using LeapDataScienceTool.Common.Énums;
using LeapDataScienceTool.Common.Services;
using LeapDataScienceTool.Components.ModelingProcess;
using LeapDataScienceTool.PageManagers;
using LeapDataScienceTool.ProgramSetup;
using LeapDataScienceTool.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor;
using Test.Leap.UITest.ExtensionMethods;

namespace Test.Leap.UITest.UC2_ModelTrainingConfig
{
    public class TestModelConfigForm : TestContext
    {
        private readonly Mock<IMudDialogInstance> _dialogInstance;
        private readonly Mock<IModelService> _modelProxyService;
        private readonly Mock<IServerAPI> _serverApi;

        public TestModelConfigForm()
        {
            _dialogInstance = new Mock<IMudDialogInstance>();
            _modelProxyService = new Mock<IModelService>();
            _serverApi = new Mock<IServerAPI>();
        }
        private void LoadServices()
        {
            Services.AddSingleton<ResponseService>();
            Services.RegisterProxyServices();
            Services.AddHttpClient("ServerClient", config =>
            {
                config.BaseAddress = new Uri("https://localhost:8000");
                config.Timeout = new TimeSpan(1, 30, 00);
                config.DefaultRequestHeaders.Clear();
            });
            Services.RegisterUIComponents();
            Services.RegisterRuntimeClasses();
            Services.AddScoped<IWorkspaceManager, WorkspaceManager>();
            Services.AddScoped<IAlgorithmComponentBuilder, AlgorithmBuilder>();
        }

        // BZ-17
        [Fact]
        public void TestComponentLoadWhenModelConfigDoesNotExists()
        {
            // Arrange
            var configDTO = new ModelConfigDTO()
            {
                ParentWorkspaceGuid = Guid.NewGuid(),
                AlgorithmParameterDTO = new LinearRegressionDTO(),
                ModelConfigGuid = Guid.NewGuid(),
            };
            _modelProxyService.Setup(meth => meth
            .GetModelConfig(It.IsAny<Guid>()))
                .ReturnsAsync((ModelConfigDTO?)null);

            LoadServices();
            _serverApi.Setup(sapi => sapi.Post<ModelConfigDTO>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(configDTO);
            // Act
            var Comp = RenderComponent<ModelConfigDialog>(param =>
            {
                param.Add(cp => cp.MudDialog, _dialogInstance.Object);
                param.Add(cp => cp.modelConfig, configDTO);
            });

            Assert.Equal(DataProcesState.CONCEPT, Comp.Instance.State);
        }

        // BZ-18
        [Fact]
        public void TestComponentLoadWhenModelConfigExists()
        {
            // Arrange
            var configDTO = new ModelConfigDTO()
            {
                ParentWorkspaceGuid = Guid.NewGuid(),
                AlgorithmParameterDTO = new LinearRegressionDTO(),
                ModelConfigGuid = Guid.NewGuid(),
            };
            LoadServices();
            _serverApi.Setup(sapi => sapi.Get<ModelConfigDTO>(It.IsAny<string>())).ReturnsAsync(configDTO);
            Services.AddSingleton(_serverApi.Object);
            var Comp = RenderComponent<ModelConfigDialog>(param =>
            {
                param.Add(cp => cp.MudDialog, _dialogInstance.Object);
                param.Add(cp => cp.modelConfig, configDTO);
            });

            Assert.Equal(DataProcesState.SET, Comp.Instance.State);
        }

        // BZ-21
        [Fact]
        public async Task TestIfSelectionSVMEnumAssignsSVMDTO()
        {
            // Arrange
            var configDTO = new ModelConfigDTO()
            {
                ParentWorkspaceGuid = Guid.NewGuid(),
                AlgorithmParameterDTO = new LinearRegressionDTO(),
                ModelConfigGuid = Guid.NewGuid(),
            };
            LoadServices();
            Services.AddScoped<IAlgorithmComponentBuilder, AlgorithmBuilder>();
            _modelProxyService.Setup(meth => meth.GetModelConfig(It.IsAny<Guid>()))
                .ReturnsAsync(configDTO);

            var Comp = RenderComponent<ModelConfigDialog>(param =>
            {
                param.Add(cp => cp.MudDialog, _dialogInstance.Object);
                param.Add(cp => cp.modelConfig, configDTO);
            });
            // Act
            // Act
            await Comp.InvokeAsync(() => Comp.Instance.SelectModelAlgorithm(ModelAlgorithm.SVMREGRESSION));
            // Assert
            Assert.IsType<SVMDTO>(Comp.Instance.modelConfig.AlgorithmParameterDTO);
        }

        // BZ-20
        [Fact]
        public async Task TestIfSelectionSVMEnumAssignsLinearRegressionDTO()
        {
            // Arrange
            // Arrange
            var configDTO = new ModelConfigDTO()
            {
                ParentWorkspaceGuid = Guid.NewGuid(),
                AlgorithmParameterDTO = new LinearRegressionDTO(),
                ModelConfigGuid = Guid.NewGuid(),
            };
            LoadServices();
            _modelProxyService.Setup(meth => meth.GetModelConfig(It.IsAny<Guid>()))
                .ReturnsAsync(configDTO);

            var Comp = RenderComponent<ModelConfigDialog>(param =>
            {
                param.Add(cp => cp.MudDialog, _dialogInstance.Object);
                param.Add(cp => cp.modelConfig, configDTO);
            });
            // Act
            await Comp.InvokeAsync(() => Comp.Instance.SelectModelAlgorithm(ModelAlgorithm.LINEAR_REGRESSION));

            // Assert
            Assert.IsType<LinearRegressionDTO>(Comp.Instance.modelConfig.AlgorithmParameterDTO);
        }

        // BZ-22
        [Fact]
        public async Task TestSubmissionOnConcept()
        {
            // Test if register function is triggered.
            // Arrange
            var configDTO = new ModelConfigDTO()
            {
                ParentWorkspaceGuid = Guid.NewGuid(),
                AlgorithmParameterDTO = new LinearRegressionDTO(),
                ModelConfigGuid = Guid.NewGuid(),
            };
            LoadServices();

            _serverApi.Setup(sapi => sapi.Get<ModelConfigDTO>(It.IsAny<string>())).ReturnsAsync((ModelConfigDTO?)null);
            _serverApi.Setup(sapi => sapi.Post<ModelConfigDTO>(It.IsAny<string>(), It.IsAny<ModelConfigDTO>())).ReturnsAsync(configDTO);
            Services.AddSingleton(_serverApi.Object);

            var Comp = RenderComponent<ModelConfigDialog>(param =>
            {
                param.Add(cp => cp.MudDialog, _dialogInstance.Object);
                param.Add(cp => cp.modelConfig, configDTO);
            });
            // Act
            await Comp.Instance.Confirm();
            // Assert
            _serverApi.Verify(m => m.Post<ModelConfigDTO>(It.IsAny<string>(), It.IsAny<ModelConfigDTO>()), Times.Once);
        }

        // BZ-23
        [Fact]
        public async Task TestSubmissionOnExisting()
        {
            // Test if register function is triggered.
            // Arrange
            var configDTO = new ModelConfigDTO()
            {
                ParentWorkspaceGuid = Guid.NewGuid(),
                AlgorithmParameterDTO = new LinearRegressionDTO(),
                ModelConfigGuid = Guid.NewGuid(),
            };
            LoadServices();

            _serverApi.Setup(sapi => sapi.Get<ModelConfigDTO>(It.IsAny<string>())).ReturnsAsync(configDTO);
            _serverApi.Setup(sapi => sapi.Put<ModelConfigDTO>(It.IsAny<string>(), It.IsAny<ModelConfigDTO>())).ReturnsAsync(configDTO);
            Services.AddSingleton(_serverApi.Object);


            var Comp = RenderComponent<ModelConfigDialog>(param =>
            {
                param.Add(cp => cp.MudDialog, _dialogInstance.Object);
                param.Add(cp => cp.modelConfig, configDTO);
            });
            // Act
            await Comp.Instance.Confirm();
            // Assert
            _serverApi.Verify(m => m.Put<ModelConfigDTO>(It.IsAny<string>(), It.IsAny<ModelConfigDTO>()), Times.Once);
        }
    }
}
