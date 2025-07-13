using Moq;
using Moq.Protected;

namespace UnitTest.Infraestructure.Repository.Base;
public static class MockHttpClientFactory
{
    public static HttpClient CreateMockHttpClient(HttpResponseMessage response, Mock<HttpMessageHandler> messageHandlerMock)
    {
        messageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var client = new HttpClient(messageHandlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        return client;
    }

    public static IHttpClientFactory CreateMockFactory(HttpClient httpClient)
    {
        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient("PersistanceClientApi")).Returns(httpClient);
        return factoryMock.Object;
    }
}
