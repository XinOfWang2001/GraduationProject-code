using Leap.ApplicationServices.DTO.DataResult;
using Leap.ApplicationServices.DTO.ModelingProcess;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.ApplicationServices.Interfaces.Creational;
using Leap.ApplicationServices.Interfaces.ExternalServiceAPI;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.ModelConfig.Enums;
using Leap.Domain.Domain.ModelStorage;
using LeapDataScienceAPI.Controllers;
using LeapDataScienceAPI.Services.Proxies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test.Leap.Backend.UnitTests
{
    public class ModelOperationControllerTest
    {
        private readonly Mock<IModelOperationService> _modelOperationServiceMock;
        private readonly ModelOperationController Controller;
        public ModelOperationControllerTest()
        {
            _modelOperationServiceMock = new Mock<IModelOperationService>();
            Controller = new ModelOperationController(_modelOperationServiceMock.Object);
        }

        // Code: A-22, NA-23
        [Fact]
        public async Task TestInvalidRequest()
        {
            string mockError = "Entiteit does not exist";
            _modelOperationServiceMock.Setup(service => service.TriggerModelTraining(It.IsAny<ModelTrainingRequestDTO>())).Throws(new InvalidDataException(mockError));
            var result = await Controller.TrainModel(It.IsAny<ModelTrainingRequestDTO>());
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // Code: A-23
        [Fact]
        public async Task TestValidRequest()
        {
            _modelOperationServiceMock.Setup(service => service.TriggerModelTraining(It.IsAny<ModelTrainingRequestDTO>())).ReturnsAsync(It.IsAny<ModelResultDataDTO>());
            var result = await Controller.TrainModel(It.IsAny<ModelTrainingRequestDTO>());
            Assert.IsType<OkObjectResult>(result.Result);
        }

        // Code: A-42, NA-24
        [Fact]
        public async Task TestControllerByInvalidRequest()
        {
            // Simulate failed request to Python service.
            _modelOperationServiceMock.Setup(service => service.TriggerModelStorage(It.IsAny<ModelStorageCreationRequestDTO>())).Throws<InvalidOperationException>();
            Controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };
            var result = await Controller.StoreModel(It.IsAny<ModelStorageCreationRequestDTO>());
            Assert.Null(result);
        }

        // Code: A-43
        [Fact]
        public async Task TestControllerByValidRequest()
        {
            Guid WorkspaceGuid = Guid.NewGuid();
            // Simulate failed request to Python service.
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
            _modelOperationServiceMock.Setup(service => service.TriggerModelStorage(It.IsAny<ModelStorageCreationRequestDTO>())).ReturnsAsync(returnValue);
            var result = await Controller.StoreModel(It.IsAny<ModelStorageCreationRequestDTO>());
            Assert.IsType<ModelStorageDTO>(result);
            Assert.Equal("test_model", result.ModelName);
            Assert.Equal($"/dev/{WorkspaceGuid}_{ModelType.FORECASTING}_model.pkl", result.ModelAddress);
        }

        // A-46 
        [Fact]
        public async Task TestControllerGetValid()
        {
            Guid WorkspaceGuid = Guid.NewGuid();
            ModelStorageAdress returnValue = new()
            {
                CreationDate = DateTime.Now,
                ModelStorageAddress = $"/dev/{WorkspaceGuid}_{ModelType.FORECASTING}_model.pkl",
                ModelAlgorithm = ModelAlgorithm.LINEAR_REGRESSION,
                ModelType = ModelType.FORECASTING,
                ModelStorageName = "test_model",
                ModelStorageVersion = "latest",
                ParentWorkspace = new()
                {
                    WorkspaceGuid = WorkspaceGuid,
                }
            };
            IPythonFacadeService python = new Mock<IPythonFacadeService>().Object;
            IWorkspaceRepository workspaceRepository = new Mock<IWorkspaceRepository>().Object;
            IModelConfigBuilder modelConfigBuilder = new Mock<IModelConfigBuilder>().Object;
            ICalculationRepository calculationRepository = new Mock<ICalculationRepository>().Object;
            var repo = new Mock<IModelStorageRepository>();
            IModelOperationService operationService = new ModelOperationService(python, workspaceRepository, calculationRepository, modelConfigBuilder, repo.Object);
            ModelOperationController controller = new(operationService);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };
            repo.Setup(r => r.GetByWorkspace(It.IsAny<Guid>())).ReturnsAsync(returnValue);
            var result = await controller.Get(It.IsAny<Guid>());

            Assert.NotNull(result);
        }

        // A-47
        [Fact]
        public async Task TestControllerGetNotFound()
        {
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
            IPythonFacadeService python = new Mock<IPythonFacadeService>().Object;
            IWorkspaceRepository workspaceRepository = new Mock<IWorkspaceRepository>().Object;
            IModelConfigBuilder modelConfigBuilder = new Mock<IModelConfigBuilder>().Object;
            ICalculationRepository calculationRepository = new Mock<ICalculationRepository>().Object;
            var repo = new Mock<IModelStorageRepository>();
            IModelOperationService operationService = new ModelOperationService(python, workspaceRepository, calculationRepository, modelConfigBuilder, repo.Object);
            ModelOperationController controller = new(operationService);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };
            repo.Setup(r => r.GetByWorkspace(It.IsAny<Guid>())).ReturnsAsync((ModelStorageAdress?)null);
            var result = await controller.Get(It.IsAny<Guid>());

            Assert.Null(result);
        }
    }
}
