using Leap.ApplicationServices.AppGeneralServices.ExternalServices;
using Leap.ApplicationServices.DTO.External_Services;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.ApplicationServices.Interfaces.ExternalServiceAPI;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.DataSource;
using LeapDataScienceAPI.Controllers;
using LeapDataScienceAPI.Services.Proxies;
using Moq;

namespace Test.Leap.Backend.UnitTests
{
    public class DataSourceControllerTest
    {
        public DataSourceController DataSourceController { get; set; }
        private readonly Mock<ISwecoWebServices<IWAWebService>> mockIwaService;
        private readonly Mock<IPreviewDataService> mockDataService;
        private readonly Mock<IProjectRepository> mockProjectRepository;
        private readonly DataSourceService dataSourceServiceHandler;

        public DataSourceControllerTest()
        {
            var mockDataSourceRepo = new Mock<IDataSourceRepo<SwecoDataSource>>();
            mockIwaService = new Mock<ISwecoWebServices<IWAWebService>>();
            dataSourceServiceHandler = new DataSourceService(mockDataSourceRepo.Object);
            mockDataService = new Mock<IPreviewDataService>();
            mockProjectRepository = new Mock<IProjectRepository>();
            DataSourceController = new DataSourceController(dataSourceServiceHandler, mockIwaService.Object, mockDataService.Object, mockProjectRepository.Object);
        }

        private List<Project> GetDummyProjects()
        {
            List<Project> _projects = new List<Project>()
            {
                new()
                {
                    Id = 1,
                    ProjectGuid = new Guid("77d3c0ea-91b5-4e6f-9e1e-f2937edfd167"),
                    Name = "SKT",
                    HumanReadableName = "DKT"
                },
                new()
                {
                    Id= 2,
                    ProjectGuid = new Guid("48fed4ee-b6a0-4c7e-88de-4e98fa39c058"),
                    Name = "SBS",
                    HumanReadableName = "BBS"
                }
            };
            return _projects;
        }

        private List<SwecoDataSource> GetDummyDataSources()
        {
            return new List<SwecoDataSource>()
            {
                new IWADataSource()
                {
                    DataSourceId = 1,
                    DataSourceGUIDId = new Guid("d367b945-191f-4ea9-8856-d12964fdd153"),
                    SourceName = "API",
                    TypeOfSource = "WEB-API",
                    Projects = GetDummyProjects(),
                },
                new IWADataSource()
                {
                    DataSourceId = 2,
                    DataSourceGUIDId = new Guid("1e337fec-5487-4d82-8249-884f130c3e94"),
                    SourceName = "IOT_HUB_Server",
                    TypeOfSource = "WEB-API",
                }
            };
        }

        private MonitorInfoDTO GetDummyMonitoringValues()
        {
            MonitorObservationData monitorObservationData = new MonitorObservationData() { Id = 1, Name = "Dummy", ValueTypeIds = [1] };
            MonitorInfoValueType monitorInfoValue = new MonitorInfoValueType() { Id = 1, Name = "Value", Quantity = "Quantity", UnitAbbr = "Val" };
            return new MonitorInfoDTO()
            {
                Observations = [monitorObservationData],
                Valuetypes = [monitorInfoValue]
            };
        }

        // Code: A-10
        [Fact]
        public async Task TestIfMonitoringValuesAreRetrieved()
        {
            // Setup.
            var datasource = GetDummyDataSources()[0];
            var firstProject = datasource.Projects[0];
            mockIwaService.Setup((service) => service.GetInfo(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(GetDummyMonitoringValues());
            mockProjectRepository.Setup(service => service.Get(1)).Returns(firstProject);
            // Act
            var result = await DataSourceController.GetObservationDataDTOsAsync(1);

            Assert.Equal(200, result.StatusCode);
        }
        // Code: A-9, N-A-21
        [Fact]
        public async Task TestProjectNotFound()
        {
            // Setup.
            var datasource = GetDummyDataSources()[0];
            var firstProject = datasource.Projects[0];
            mockIwaService.Setup((service) => service.GetInfo(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(GetDummyMonitoringValues());
            // Act
            var result = await DataSourceController.GetObservationDataDTOsAsync(3222);

            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Project is niet gevonden", result.Message);
        }

        // Code: A-8, NA-22
        [Fact]
        public async Task TestWhenPreviewDataRequestFails()
        {
            // Functional requirement: Model training, Data source selection.
            // Testcase: Ensure that the controller handles exceptions when the data service fails to retrieve preview data.
            // Arrange
            mockDataService.Setup((proxy) => proxy.GetPreviewData(It.IsAny<Guid>(), It.IsAny<bool>())).ThrowsAsync(new Exception("Preview data request failed"));
            var result = await DataSourceController.GetData(Guid.NewGuid());
            var specific = result.Result;
            // Assert
            Assert.Equal("NotFoundObjectResult", specific.GetType().Name);

        }
    }
}
