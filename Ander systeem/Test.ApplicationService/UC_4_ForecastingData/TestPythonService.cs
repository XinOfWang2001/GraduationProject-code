using Leap.ApplicationServices.AppGeneralServices.ExternalServices;
using Leap.ApplicationServices.DTO.DataResult;
using Leap.ApplicationServices.DTO.ModelingProcess;
using Leap.ApplicationServices.Interfaces.ExternalServiceAPI;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;

namespace Test.ApplicationService.UC_4_ForecastingData
{
    public class TestPythonService
    {
        public readonly Mock<IHttpClientFactory> httpClientFactory;
        public readonly IPythonFacadeService pythonFacadeService;

        public TestPythonService()
        {
            httpClientFactory = new Mock<IHttpClientFactory>();
            pythonFacadeService = new LeapFastDSAPIService(httpClientFactory.Object);
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
                    Content = new StringContent(jsonObject, Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            httpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
            return httpClientFactory.Object;
        }

        /// <summary>
        /// A-33
        /// Use case: UC-4, Forecasting data generated
        /// Testcase: Validate if requested JSON Format is parsed into a correct C# DTO
        /// Expected result: Successfull mapping
        /// </summary>
        [Fact]
        public async Task TestSuccessfullResult()
        {
            string jsonString = File.ReadAllText("DummyJSON/TestForecastModelData.json");
            var mockFactory = GetMockHttpFactory(jsonString, HttpStatusCode.OK);

            LeapFastDSAPIService service = new(mockFactory);
            var result = await service.TriggerModelTraining(It.IsAny<ModelRequestDTO>());

            Assert.IsType<ModelResultDataDTO>(result);
            Assert.Equal("X_1", result.DataSet.ColumnNames.ElementAt(0));
            Assert.Equal("X_2", result.DataSet.ColumnNames.ElementAt(1));
            Assert.Equal("X_1-predicted", result.PredictionSet.ColumnNames.ElementAt(0));
            Assert.Equal("X_2-predicted", result.PredictionSet.ColumnNames.ElementAt(1));

            Assert.Equal([15.1f, 18.5f], result.DataSet.Values["X_1"]);
            Assert.Equal([25.1f, 28.5f], result.DataSet.Values["X_2"]);
            Assert.Equal([9.1f, 12.5f], result.PredictionSet.Values["X_1-predicted"]);
            Assert.Equal([35.1f, 38.5f], result.PredictionSet.Values["X_2-predicted"]);
        }

        /// A-34, NA-20
        [Fact]
        public async Task TestFailedRequestRaisesError()
        {
            string jsonString = "Error";
            var mockFactory = GetMockHttpFactory(jsonString, HttpStatusCode.BadRequest);

            LeapFastDSAPIService service = new LeapFastDSAPIService(mockFactory);
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.TriggerModelTraining(It.IsAny<ModelRequestDTO>()));
        }

        //// A-41b, NA-19B
        /// Use-case: UC-3 Model storage
        /// 
        [Fact]
        public async Task TestFailedModelStorage()
        {
            string jsonString = "Error";
            var mockFactory = GetMockHttpFactory(jsonString, HttpStatusCode.BadRequest);

            LeapFastDSAPIService service = new LeapFastDSAPIService(mockFactory);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.StoreModel(It.IsAny<ModelRequestDTO>()));
        }


    }
}
