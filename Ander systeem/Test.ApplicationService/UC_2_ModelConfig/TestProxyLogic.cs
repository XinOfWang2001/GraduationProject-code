using Leap.ApplicationServices.AppGeneralServices.ExternalServices;
using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.DTO.DataResult;
using Leap.ApplicationServices.Interfaces.ExternalServiceAPI;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.DataConfig;
using Leap.Domain.Domain.DataSource;
using LeapDataScienceAPI.Services.Proxies;
using Moq;
using Moq.Protected;
using System.Net;

namespace Test.ApplicationService.UC_2_ModelConfig
{
    public class TestProxyLogic
    {
        public readonly PreviewDataService proxyService;
        public readonly Mock<IDataExtractRepository> extractRepository;
        public readonly Mock<IPythonFacadeService> pythonService;
        public readonly Mock<IHttpClientFactory> httpClientFactory;
        public readonly LeapFastDSAPIService concretePythonService;

        public TestProxyLogic()
        {
            // Initialize any dependencies or services here if needed
            extractRepository = new Mock<IDataExtractRepository>();
            pythonService = new Mock<IPythonFacadeService>();
            httpClientFactory = new Mock<IHttpClientFactory>();
            concretePythonService = new LeapFastDSAPIService(httpClientFactory.Object);
            proxyService = new PreviewDataService(extractRepository.Object, pythonService.Object);
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

        // A-20B, N-A-13
        [Fact]
        public async Task TestIfNonExistentExtractObjectRaisesError()
        {
            extractRepository.Setup(repo => repo.Get(It.IsAny<Guid>()))
                .Returns((DataExtracter?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await proxyService.GetPreviewData(Guid.NewGuid(), false));
        }

        // A-19
        [Fact]
        public async Task TestSuccesfullRequestProcess()
        {
            // Arrange
            string jsonResponse = File.ReadAllText("DummyJSON/TestResponsePythonPreviewData.json");
            PreviewDataDTO previewDataDTO = System.Text.Json.JsonSerializer.Deserialize<PreviewDataDTO>(jsonResponse)!;
            DataExtracter data = new()
            {
                ProcessId = Guid.NewGuid(),
                ParentWorkspace = new() { WorkspaceGuid = Guid.NewGuid() },
                DataSourceConfig = new()
                {
                    DataPoints = 1000,
                    StartDate = DateTime.Now.AddDays(-30),
                    EndDate = DateTime.Now,
                    TimeLevel = 1,
                    TimelevelName = "1 second",
                    TimelevelRange = 10000,
                    ValueTypes =
                    [
                        new ValueTypes { ValueTypeId = 1, ValueTypeName = "DummyVT1" },
                        new ValueTypes { ValueTypeId = 2, ValueTypeName = "DummyVT2" }
                    ],
                    Sensors = [
                        new SensorObject { SensorId = 1, SensorName = "DummyObservation1" },
                        new SensorObject { SensorId = 2, SensorName = "DummyObservation2" }
                        ],
                    AssignedProject = new Project()
                    {
                        ProjectGuid = Guid.NewGuid(),
                        Name = "Test_Project",
                        HumanReadableName = "Test project",
                        ProjectToken = "test-token",
                        SwecoDataSource = new IWADataSource()
                        {
                            SourceName = "Localsource",
                        },
                    },
                }
            };
            extractRepository.Setup(repo => repo.GetByWorkspace(It.IsAny<Guid>()))
                .Returns(data);

            pythonService.Setup(service => service.RequestPreviewData(It.IsAny<DataRequestDTO>())).ReturnsAsync(previewDataDTO);

            // Act
            var result = await proxyService.GetPreviewData(Guid.NewGuid(), true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.DataColumns.Count()); // Assuming 5 columns in the DataFrame
        }

        // A-20, NA-14
        [Fact]
        public async Task TestRetrievalFailed()
        {
            // Arrange
            var mockFactory = GetMockHttpFactory("Error", HttpStatusCode.UnprocessableEntity);
            // Mock return value
            var leapService = new LeapFastDSAPIService(mockFactory);
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await leapService.RequestPreviewData(It.IsAny<DataRequestDTO>()));
        }

        // UC-4
        // Code: // A-19B
        [Fact]
        public async Task TestRetrievalSuccesfull()
        {
            // Arrange
            string jsonString = File.ReadAllText("DummyJSON/TestResponsePythonPreviewData.json");
            var mockFactory = GetMockHttpFactory(jsonString, HttpStatusCode.OK);
            // Mock return value
            var leapService = new LeapFastDSAPIService(mockFactory);
            // Act & Assert
            var result = await leapService.RequestPreviewData(It.IsAny<DataRequestDTO>());

            Assert.Equal(3, result?.DataColumns.Count());
            Assert.Equal(4, result?.DataCount);
            Assert.NotEmpty(result?.DataSet.Values);
        }
    }
}
