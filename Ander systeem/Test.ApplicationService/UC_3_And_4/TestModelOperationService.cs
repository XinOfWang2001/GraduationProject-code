using Leap.ApplicationServices.AppGeneralServices.ExternalServices;
using Leap.ApplicationServices.DTO.ModelingProcess;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.ApplicationServices.Interfaces.Creational;
using Leap.ApplicationServices.Interfaces.ExternalServiceAPI;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.DataConfig;
using Leap.Domain.Domain.DataSource;
using Leap.Domain.Domain.ModelConfig.ModelParams;
using Leap.Domain.Domain.ModelStorage;
using Leap.Domain.Domain.Workspaces;
using LeapDataScienceAPI.ProgramSetup;
using LeapDataScienceAPI.Services.BuilderAndMappers.ModelBuilders;
using LeapDataScienceAPI.Services.Proxies;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using System.Net;
using ModelConfiguration = Leap.Domain.Domain.ModelConfig.ModelConfiguration;

namespace Test.ApplicationService.UC_3_And_4
{
    public class TestModelOperationService
    {
        private readonly Mock<IPythonFacadeService> pythonFacadeService;
        private readonly Mock<IWorkspaceRepository> workspaceRepository;
        private readonly Mock<IModelStorageRepository> modelStorageRepository;
        private readonly Mock<IHttpClientFactory> httpClientFactory;
        private readonly IModelConfigBuilder modelConfigBuilder;
        private readonly IModelOperationService ModelOperationService;
        private readonly Mock<ICalculationRepository> CalculationRepository;

        public TestModelOperationService()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.RegisterRuntimeClasses();
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            pythonFacadeService = new Mock<IPythonFacadeService>();
            workspaceRepository = new Mock<IWorkspaceRepository>();
            modelStorageRepository = new Mock<IModelStorageRepository>();
            modelStorageRepository = new Mock<IModelStorageRepository>();
            httpClientFactory = new Mock<IHttpClientFactory>();
            CalculationRepository = new Mock<ICalculationRepository>();
            modelConfigBuilder = new ModelConfigBuilder(serviceProvider);
            ModelOperationService = new ModelOperationService(
                pythonFacadeService.Object,
                workspaceRepository.Object,
                CalculationRepository.Object,
                modelConfigBuilder,
                modelStorageRepository.Object);
        }
        private IHttpClientFactory GetMockHttpFactory(string jsonObject, HttpStatusCode statusCode)
        {
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(jsonObject, System.Text.Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            httpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
            return httpClientFactory.Object;
        }

        // A-36 NA-15
        [Fact]
        public async Task ExpectExceptionsWhenWorkspaceIsNotFound()
        {
            ModelTrainingRequestDTO modelRequestDTO = new()
            {
                ModelConfigGuid = Guid.NewGuid(),
                DataExtractConfigGuid = Guid.NewGuid(),
                WorkspaceGuid = Guid.NewGuid(),
            };
            workspaceRepository.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns((Workspace?)null);
            pythonFacadeService.Setup(repo => repo.TriggerModelTraining(It.IsAny<ModelRequestDTO>())).ThrowsAsync(new Exception("Fail"));

            await Assert.ThrowsAsync<InvalidDataException>(() => ModelOperationService.TriggerModelTraining(modelRequestDTO));
        }

        // A-37, NA-17
        [Fact]
        public async Task ExpectExceptionsWhenDataExtractionIsNotPresent()
        {
            ModelTrainingRequestDTO modelRequestDTO = new()
            {
                ModelConfigGuid = Guid.NewGuid(),
                DataExtractConfigGuid = Guid.NewGuid(),
                WorkspaceGuid = Guid.NewGuid(),
            };
            Workspace workspace = new()
            {
                WorkspaceGuid = Guid.NewGuid(),
                DataExtraction = null,
            };

            workspaceRepository.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(workspace);
            pythonFacadeService.Setup(repo => repo.TriggerModelTraining(It.IsAny<ModelRequestDTO>())).ThrowsAsync(new Exception("Fail"));

            await Assert.ThrowsAsync<InvalidDataException>(() => ModelOperationService.TriggerModelTraining(modelRequestDTO));
        }

        // A-38, NA-16
        [Fact]
        public async Task ExpectExceptionsWhenModelConfigIsNotPresent()
        {
            ModelTrainingRequestDTO modelRequestDTO = new()
            {
                ModelConfigGuid = Guid.NewGuid(),
                DataExtractConfigGuid = Guid.NewGuid(),
                WorkspaceGuid = Guid.NewGuid(),
            };
            Workspace workspace = new()
            {
                WorkspaceGuid = Guid.NewGuid(),
                DataExtraction = new() { DataSourceConfig = new DataSourceConfig() { AssignedProject = new() { Name = "test", HumanReadableName = "Test project" } } },
                ModelConfig = null
            };
            workspaceRepository.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(workspace);
            pythonFacadeService.Setup(repo => repo.TriggerModelTraining(It.IsAny<ModelRequestDTO>())).ThrowsAsync(new Exception("Fail"));

            await Assert.ThrowsAsync<InvalidDataException>(() => ModelOperationService.TriggerModelTraining(modelRequestDTO));
        }

        // A-35
        [Fact]
        public async Task ExpectExceptionsRequestFromAPIFails()
        {
            // Arrange
            ModelTrainingRequestDTO modelRequestDTO = new()
            {
                ModelConfigGuid = Guid.NewGuid(),
                DataExtractConfigGuid = Guid.NewGuid(),
                WorkspaceGuid = Guid.NewGuid(),
            };
            DataExtracter extracter = new()
            {
                ParentWorkspace = new(),
                DataSourceConfig = new()
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    AssignedProject = new()
                    {
                        Name = "test_project",
                        HumanReadableName = "Test project",
                        SwecoDataSource = new IWADataSource()
                        {
                            DataSourceId = 2,
                            DataSourceGUIDId = new Guid("d367b945-191f-4ea9-8856-d12964fdd153"),
                            SourceName = "API",
                            TypeOfSource = "WEB-API",
                        }
                    },
                }
            };
            Workspace workspace = new()
            {
                WorkspaceGuid = Guid.NewGuid(),
                DataExtraction = extracter,
                ModelConfig = null
            };
            ModelConfiguration ModelConfig = new() { ModelParameters = new LinearRegressionParameters(), ParentWorkspace = workspace };
            workspace.ModelConfig = ModelConfig;
            workspaceRepository.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(workspace);
            pythonFacadeService.Setup(repo => repo.TriggerModelTraining(It.IsAny<ModelRequestDTO>())).ThrowsAsync(new InvalidOperationException("Fail"));
            // Act and Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => ModelOperationService.TriggerModelTraining(modelRequestDTO));
        }

        // A-39
        // Testcase: Test of incompleet werkprocess zonder modelconfiguratie een foutmelding terugstuurt.
        // Expected result: Geeft exceptie terug.
        [Fact]
        public async Task TestIncompleteWorkspaceWithoutModelConfiguration()
        {
            // Arrange
            ModelStorageCreationRequestDTO requestDTO = new()
            {
                ModelConfigGuid = Guid.NewGuid(),
                DataExtractConfigGuid = Guid.NewGuid(),
                WorkspaceGuid = Guid.NewGuid(),
            };
            DataExtracter extracter = new()
            {
                ParentWorkspace = new(),
                DataSourceConfig = new()
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    AssignedProject = new Project()
                    {
                        Name = "test_project",
                        HumanReadableName = "Test project",
                        SwecoDataSource = new IWADataSource()
                        {
                            DataSourceId = 2,
                            DataSourceGUIDId = new Guid("d367b945-191f-4ea9-8856-d12964fdd153"),
                            SourceName = "API",
                            TypeOfSource = "WEB-API",
                        }
                    },
                }
            };
            Workspace workspace = new()
            {
                WorkspaceGuid = Guid.NewGuid(),
                DataExtraction = extracter,
                ModelConfig = null
            };
            workspaceRepository.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(workspace);

            // Act & Arrange
            var result = await Assert.ThrowsAsync<InvalidDataException>(() => ModelOperationService.TriggerModelStorage(requestDTO));
            Assert.Equal("Incomplete Workspace entiteit. DataExtractConfig & Modelconfig required for model storage", result.Message);
        }

        // A-40, NA-18
        // Testcase: Test of incompleet werkprocess zonder databron configuratie een foutmelding terugstuurt.
        // Expected result: Geeft exceptie terug.
        [Fact]
        public async Task TestIncompleteWorkspaceWithoutDataExtractConfig()
        {
            // Arrange
            ModelStorageCreationRequestDTO requestDTO = new()
            {
                ModelConfigGuid = Guid.NewGuid(),
                DataExtractConfigGuid = Guid.NewGuid(),
                WorkspaceGuid = Guid.NewGuid(),
            };
            DataExtracter extracter = new DataExtracter()
            {
                ParentWorkspace = new Workspace(),
                DataSourceConfig = new DataSourceConfig()
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    AssignedProject = new Project()
                    {
                        Name = "test_project",
                        HumanReadableName = "Test project",
                        SwecoDataSource = new IWADataSource()
                        {
                            DataSourceId = 2,
                            DataSourceGUIDId = new Guid("d367b945-191f-4ea9-8856-d12964fdd153"),
                            SourceName = "API",
                            TypeOfSource = "WEB-API",
                        }
                    },
                }
            };
            Workspace workspace = new Workspace()
            {
                WorkspaceGuid = Guid.NewGuid(),
                DataExtraction = null,
                ModelConfig = null
            };
            ModelConfiguration ModelConfig = new() { ModelParameters = new LinearRegressionParameters(), ParentWorkspace = workspace };
            workspace.ModelConfig = ModelConfig;
            workspaceRepository.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(workspace);

            // Act & Arrange
            var result = await Assert.ThrowsAsync<InvalidDataException>(() => ModelOperationService.TriggerModelStorage(requestDTO));
            Assert.Equal("Incomplete Workspace entiteit. DataExtractConfig & Modelconfig required for model storage", result.Message);
        }

        // NA-15B
        [Fact]
        public async Task TestIfWorkspaceNotFoundReturnsError()
        {
            // Arrange
            ModelStorageCreationRequestDTO requestDTO = new()
            {
                ModelConfigGuid = Guid.NewGuid(),
                DataExtractConfigGuid = Guid.NewGuid(),
                WorkspaceGuid = Guid.NewGuid(),
            };

            workspaceRepository.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns((Workspace?)null);

            // Act & Arrange
            var result = await Assert.ThrowsAsync<InvalidDataException>(() => ModelOperationService.TriggerModelStorage(requestDTO));
            Assert.Equal("Workspace not found", result.Message);
        }

        // A-41, NA-19
        [Fact]
        public async Task TestIfModelStorageFailsIfPythonSystemsFails()
        {
            // Arrange
            ModelStorageCreationRequestDTO requestDTO = new()
            {
                ModelConfigGuid = Guid.NewGuid(),
                DataExtractConfigGuid = Guid.NewGuid(),
                WorkspaceGuid = Guid.NewGuid(),
            };
            DataExtracter extracter = new()
            {
                ParentWorkspace = new(),
                DataSourceConfig = new()
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    AssignedProject = new Project()
                    {
                        Name = "test_project",
                        HumanReadableName = "Test project",
                        SwecoDataSource = new IWADataSource()
                        {
                            DataSourceId = 2,
                            DataSourceGUIDId = new Guid("d367b945-191f-4ea9-8856-d12964fdd153"),
                            SourceName = "API",
                            TypeOfSource = "WEB-API",
                        }
                    },
                }
            };
            Workspace workspace = new()
            {
                WorkspaceGuid = Guid.NewGuid(),
                DataExtraction = extracter,
                ModelConfig = null
            };
            ModelConfiguration ModelConfig = new() { ModelParameters = new LinearRegressionParameters(), ParentWorkspace = workspace };
            workspace.ModelConfig = ModelConfig;

            workspaceRepository.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(workspace);
            pythonFacadeService.Setup(python => python.StoreModel(It.IsAny<ModelRequestDTO>())).Throws<InvalidOperationException>();

            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(() => ModelOperationService.TriggerModelStorage(requestDTO));
        }

        // Code: A-44
        [Fact]
        public async Task TestSuccessfullModelStorage()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.RegisterRuntimeClasses();
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            string jsonString = File.ReadAllText("DummyJSON/TestModelStorageReturnValue.json");
            ModelStorageCreationRequestDTO requestDTO = new()
            {
                ModelConfigGuid = Guid.NewGuid(),
                DataExtractConfigGuid = Guid.NewGuid(),
                WorkspaceGuid = Guid.NewGuid(),
            };
            DataExtracter extracter = new()
            {
                ParentWorkspace = new(),
                DataSourceConfig = new()
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    AssignedProject = new()
                    {
                        Name = "test_project",
                        HumanReadableName = "Test project",
                        SwecoDataSource = new IWADataSource()
                        {
                            DataSourceId = 2,
                            DataSourceGUIDId = new Guid("d367b945-191f-4ea9-8856-d12964fdd153"),
                            SourceName = "API",
                            TypeOfSource = "WEB-API",
                        }
                    },
                }
            };
            Workspace workspace = new()
            {
                WorkspaceGuid = Guid.NewGuid(),
                DataExtraction = extracter,
                ModelConfig = null
            };
            ModelConfiguration ModelConfig = new() { ModelParameters = new LinearRegressionParameters(), ParentWorkspace = workspace };
            workspace.ModelConfig = ModelConfig;

            ModelStorageAdress address = new()
            {
                ModelStorageId = 1000,
                ModelStorageAddress = "/dev/af4ac3ed-dd79-407d-8fcd-77c984974657_FORECASTING_model.pkl",
                ModelStorageName = "test model",
                ModelStorageVersion = "latest",
                ParentWorkspace = workspace,
                ParentWorkspaceId = workspace.WorkspaceId,
                ModelAlgorithm = Leap.Domain.Domain.ModelConfig.Enums.ModelAlgorithm.LINEAR_REGRESSION,
                ModelType = Leap.Domain.Domain.ModelConfig.Enums.ModelType.FORECASTING
            };


            var mockFactory = GetMockHttpFactory(jsonString, HttpStatusCode.OK);
            LeapFastDSAPIService leap = new(mockFactory);
            workspaceRepository.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(workspace);
            modelStorageRepository.Setup(repo => repo.Create(It.IsAny<ModelStorageAdress>())).ReturnsAsync(address);
            ModelOperationService service = new(leap, workspaceRepository.Object, CalculationRepository.Object, new ModelConfigBuilder(serviceProvider), modelStorageRepository.Object);

            // Act
            var result = await service.TriggerModelStorage(requestDTO);

            // Assert 
            Assert.Equal("test model", result.ModelName);
            Assert.Equal("/dev/af4ac3ed-dd79-407d-8fcd-77c984974657_FORECASTING_model.pkl", result.ModelAddress);
            modelStorageRepository.Verify(_ => _.Create(It.IsAny<ModelStorageAdress>()), Times.Once());
        }
        // A-45 Overwriting modeladdress
        [Fact]
        public async Task TestSuccessfullModelStorageOverwrite()
        {
            string jsonString = File.ReadAllText("DummyJSON/TestModelStorageReturnValue.json");
            var mockFactory = GetMockHttpFactory(jsonString, HttpStatusCode.OK);
            ModelStorageCreationRequestDTO requestDTO = new()
            {
                ModelConfigGuid = Guid.NewGuid(),
                DataExtractConfigGuid = Guid.NewGuid(),
                WorkspaceGuid = Guid.NewGuid(),
                Overwrite = true,
            };
            DataExtracter extracter = new()
            {
                ParentWorkspace = new(),
                DataSourceConfig = new()
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    AssignedProject = new()
                    {
                        Name = "test_project",
                        HumanReadableName = "Test project",
                        SwecoDataSource = new IWADataSource()
                        {
                            DataSourceId = 2,
                            DataSourceGUIDId = new Guid("d367b945-191f-4ea9-8856-d12964fdd153"),
                            SourceName = "API",
                            TypeOfSource = "WEB-API",
                        }
                    },
                }
            };
            Workspace workspace = new()
            {
                WorkspaceGuid = Guid.NewGuid(),
                DataExtraction = extracter,
                ModelConfig = null
            };
            ModelConfiguration ModelConfig = new() { ModelParameters = new LinearRegressionParameters(), ParentWorkspace = workspace };
            workspace.ModelConfig = ModelConfig;

            ModelStorageAdress address = new ModelStorageAdress()
            {
                ModelStorageId = 1000,
                ModelStorageAddress = "/dev/af4ac3ed-dd79-407d-8fcd-77c984974657_FORECASTING_model.pkl",
                ModelStorageName = "test model",
                ModelStorageVersion = "latest",
                ParentWorkspace = workspace,
                ParentWorkspaceId = workspace.WorkspaceId,
                ModelAlgorithm = Leap.Domain.Domain.ModelConfig.Enums.ModelAlgorithm.LINEAR_REGRESSION,
                ModelType = Leap.Domain.Domain.ModelConfig.Enums.ModelType.FORECASTING
            };

            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.RegisterRuntimeClasses();
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            LeapFastDSAPIService leap = new(mockFactory);
            workspaceRepository.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(workspace);
            modelStorageRepository.Setup(repo => repo.Create(It.IsAny<ModelStorageAdress>())).ReturnsAsync(address);
            ModelOperationService service = new(leap, workspaceRepository.Object, CalculationRepository.Object, new ModelConfigBuilder(serviceProvider), modelStorageRepository.Object);

            // Act
            var result = await service.TriggerModelStorage(requestDTO);

            // Assert 
            Assert.Equal("test model", result.ModelName);
            Assert.Equal("/dev/af4ac3ed-dd79-407d-8fcd-77c984974657_FORECASTING_model.pkl", result.ModelAddress);
            modelStorageRepository.Verify(_ => _.Update(It.IsAny<Guid>(), It.IsAny<ModelStorageAdress>()), Times.Once());
        }
    }
}
