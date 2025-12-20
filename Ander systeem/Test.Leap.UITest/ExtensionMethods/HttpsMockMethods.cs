using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http.Json;

namespace Test.Leap.UITest.ExtensionMethods
{
    public class HttpsMockMethods
    {
        public static IHttpClientFactory GetDummyHttpClient(object returnBody, HttpStatusCode statusCode)
        {
            // Arrange
            var mockResponse = new HttpResponseMessage(statusCode)
            {
                Content = JsonContent.Create(returnBody)
            };

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(mockResponse);

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://fakeapi.com/")
            };

            var mockFactory = new Mock<IHttpClientFactory>();
            mockFactory.Setup(_ => _.CreateClient("ServerClient")).Returns(httpClient);

            return mockFactory.Object;
        }
    }
}
