using Bunit;
using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.DTO.ModelingProcess;
using Leap.ApplicationServices.DTO.Workspace;
using Leap.Domain.Domain.ModelConfig.Enums;
using LeapDataScienceTool.API;
using LeapDataScienceTool.Components.ModelStorage;
using LeapDataScienceTool.PageManagers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Test.Leap.UITest.ExtensionMethods;

namespace Test.Leap.UITest.UC3_ModelStorage
{
    public class TestModelStorage : TestContext
    {
        /// BZ-30
        [Fact]
        public void TestModelStorageDialogIfModelTrainingFailed()
        {
            Guid WorkspaceGuid = Guid.NewGuid();
            Mock<IWorkspaceManager> manager = new();
            Mock<IServerAPI> MockServerAPI = new();
            manager.Setup(man => man.GetDataExtractConfigDTO()).Returns(new DataExtractConfigDTO() { WorkspaceId = WorkspaceGuid });
            manager.Setup(man => man.GetWorkspaceConfigDTO()).Returns(new WorkspaceConfigDTO() { WorkspaceGuid = WorkspaceGuid });
            manager.Setup(man => man.GetModelConfig()).Returns(new ModelConfigDTO() { ParentWorkspaceGuid = WorkspaceGuid, ModelConfigGuid = Guid.NewGuid() });
            MockServerAPI.Setup(ms => ms.Get<ModelStorageDTO>(It.IsAny<string>())).ReturnsAsync((ModelStorageDTO?)null);
            MockServerAPI.Setup(ms => ms.Post<ModelStorageDTO>(It.IsAny<string>(), It.IsAny<ModelStorageCreationRequestDTO>())).ReturnsAsync((ModelStorageDTO?)null);

            Services.RegisterUIComponents();
            Services.AddSingleton(manager.Object);
            Services.AddSingleton(MockServerAPI.Object);

            var ModelComponent = RenderComponent<ModelStorageDialog>();

            //Assert
            Assert.Equal("Er is iets misgegaan bij het opslaan van het model. Controlleer even de databron en modelconfiguratie gegevens.", ModelComponent.Instance.LoadingText);
        }

        /// BZ-31
        [Fact]
        public void TestModelStorageDialogIfModelTrainingSucceeded()
        {
            // Arrange
            Guid WorkspaceGuid = Guid.NewGuid();
            ModelStorageDTO returnValue = new()
            {
                DateOfCreation = DateTime.Now,
                ModelAddress = $"/dev/{WorkspaceGuid}_{ModelType.FORECASTING}_model.pkl",
                ModelAlgorithm = ModelAlgorithm.LINEAR_REGRESSION,
                ModelType = ModelType.FORECASTING,
                ModelName = "test_model",
                ModelVersion = "latest",
                WorkspaceGuid = WorkspaceGuid
            };
            Mock<IWorkspaceManager> manager = new Mock<IWorkspaceManager>();
            Mock<IServerAPI> MockServerAPI = new();
            manager.Setup(man => man.GetDataExtractConfigDTO()).Returns(new DataExtractConfigDTO() { WorkspaceId = WorkspaceGuid });
            manager.Setup(man => man.GetWorkspaceConfigDTO()).Returns(new WorkspaceConfigDTO() { WorkspaceGuid = WorkspaceGuid });
            manager.Setup(man => man.GetModelConfig()).Returns(new ModelConfigDTO() { ParentWorkspaceGuid = WorkspaceGuid, ModelConfigGuid = Guid.NewGuid() });
            MockServerAPI.Setup(ms => ms.Get<ModelStorageDTO>(It.IsAny<string>())).ReturnsAsync((ModelStorageDTO?)null);
            MockServerAPI.Setup(ms => ms.Post<ModelStorageDTO>(It.IsAny<string>(), It.IsAny<ModelStorageCreationRequestDTO>())).ReturnsAsync(returnValue);

            Services.RegisterUIComponents();
            Services.AddSingleton(manager.Object);
            Services.AddSingleton(MockServerAPI.Object);
            // Act
            var ModelComponent = RenderComponent<ModelStorageDialog>();

            //Assert
            Assert.Equal("Model is succesvol opgeslagen.", ModelComponent.Instance.SuccesfullText);
        }

        /// BZ-32, N-BZ-38a Gebruiksvriendelijkheid.
        [Fact]
        public async Task TestModelStorageDialogIfOverwritingModelSucceeds()
        {
            // Arrange
            Guid WorkspaceGuid = Guid.NewGuid();
            ModelStorageDTO returnValue = new()
            {
                DateOfCreation = DateTime.Now,
                ModelAddress = $"/dev/{WorkspaceGuid}_{ModelType.FORECASTING}_model.pkl",
                ModelAlgorithm = ModelAlgorithm.LINEAR_REGRESSION,
                ModelType = ModelType.FORECASTING,
                ModelName = "test_model",
                ModelVersion = "latest",
                WorkspaceGuid = WorkspaceGuid
            };
            Mock<IWorkspaceManager> manager = new Mock<IWorkspaceManager>();
            Mock<IServerAPI> MockServerAPI = new();
            manager.Setup(man => man.GetDataExtractConfigDTO()).Returns(new DataExtractConfigDTO() { WorkspaceId = WorkspaceGuid });
            manager.Setup(man => man.GetWorkspaceConfigDTO()).Returns(new WorkspaceConfigDTO() { WorkspaceGuid = WorkspaceGuid });
            manager.Setup(man => man.GetModelConfig()).Returns(new ModelConfigDTO() { ParentWorkspaceGuid = WorkspaceGuid, ModelConfigGuid = Guid.NewGuid() });
            // Returns existing model
            MockServerAPI.Setup(ms => ms.Get<ModelStorageDTO>(It.IsAny<string>())).ReturnsAsync(returnValue);
            MockServerAPI.Setup(ms => ms.Post<ModelStorageDTO>(It.IsAny<string>(), It.IsAny<ModelStorageCreationRequestDTO>())).ReturnsAsync(returnValue);

            Services.RegisterUIComponents();
            Services.AddSingleton(manager.Object);
            Services.AddSingleton(MockServerAPI.Object);
            // Act
            var ModelComponent = RenderComponent<ModelStorageDialog>();
            Assert.Equal("Weet je zeker dat je het huidige model wil overschrijven?", ModelComponent.Instance.LoadingText);

            await ModelComponent.Instance.OverwriteModel();
            //Assert
            Assert.Equal("Model is succesvol opgeslagen.", ModelComponent.Instance.SuccesfullText);
        }

        /// BZ-32, N-BZ-38 Gebruiksvriendelijkheid.
        [Fact]
        public async Task TestIfInCompleteWorkspaceConfigCausesError()
        {
            Mock<IWorkspaceManager> manager = new Mock<IWorkspaceManager>();
            manager.Setup(ms => ms.GetDataExtractConfigDTO()).Returns((DataExtractConfigDTO?)null);
            manager.Setup(ms => ms.GetModelConfig()).Returns((ModelConfigDTO?)null);
            Mock<IServerAPI> MockServerAPI = new();
            Services.RegisterUIComponents();
            Services.AddSingleton(manager.Object);
            Services.AddSingleton(MockServerAPI.Object);
            var Component = RenderComponent<ModelStorageDialog>();

            Assert.Equal("Stel eerst een databron en model in, voordat een model opgeslagen wordt.", Component.Instance.LoadingText);
        }
    }
}
