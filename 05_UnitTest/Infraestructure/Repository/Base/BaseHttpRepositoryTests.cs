using Moq;
using Moq.Protected;

namespace UnitTest.Infraestructure.Repository.Base;
public abstract class BaseHttpRepositoryTests
{
    protected HttpClient CreateMockHttpClient(HttpResponseMessage response)
    {
        var messageHandlerMock = new Mock<HttpMessageHandler>();

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

    protected IHttpClientFactory CreateMockFactory(HttpClient httpClient)
    {
        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient("PersistanceClientApi")).Returns(httpClient);
        return factoryMock.Object;
    }
}
