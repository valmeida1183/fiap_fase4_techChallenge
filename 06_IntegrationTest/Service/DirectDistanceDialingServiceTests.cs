using Application.Service;
using Core.Entity;
using Infraestructure.Repository;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http.Json;
using UnitTest.Infraestructure.Repository.Base;

namespace IntegrationTest.Service;
public class DirectDistanceDialingServiceTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetByIdAsync_ExistingId_ReturnsDirectDistanceDialing()
    {
        //Arrange
        var dddCode = 11;  
        var ddd = new DirectDistanceDialing { Id = dddCode, Region = "São Paulo", CreatedOn = DateTime.Now };

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(ddd)
        };

        var messageHandlerMock = new Mock<HttpMessageHandler>();
        var mockHandler = MockHttpClientFactory.CreateMockHttpClient(response, messageHandlerMock);
        var mockHttpClientFactory = MockHttpClientFactory.CreateMockFactory(mockHandler);
               
        var dddRepository = new DirectDistanceDialingHttpRepository(mockHttpClientFactory);

        var dddService = new DirectDistanceDialingService(dddRepository);

        // Act
        var result = await dddService.GetByIdAsync(dddCode);

        // Assert
        messageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get && req.RequestUri.ToString().EndsWith($"/persistence-api/v1/ddd/{dddCode}")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        //Arrange
        var dddCode = 999;
        var ddd = new DirectDistanceDialing { Id = dddCode, Region = "São Paulo", CreatedOn = DateTime.Now };

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NoContent
        };

        var messageHandlerMock = new Mock<HttpMessageHandler>();
        var mockHandler = MockHttpClientFactory.CreateMockHttpClient(response, messageHandlerMock);
        var mockHttpClientFactory = MockHttpClientFactory.CreateMockFactory(mockHandler);

        var dddRepository = new DirectDistanceDialingHttpRepository(mockHttpClientFactory);

        var dddService = new DirectDistanceDialingService(dddRepository);

        // Act
        var result = await dddService.GetByIdAsync(dddCode);

        // Assert
        messageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get && req.RequestUri.ToString().EndsWith($"/persistence-api/v1/ddd/{dddCode}")),
            ItExpr.IsAny<CancellationToken>());
    }
}
