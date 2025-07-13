using Application.Service;
using Core.Entity;
using Infraestructure.Repository;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http.Json;
using UnitTest.Infraestructure.Repository.Base;

namespace IntegrationTest.Service;
public class ContactServiceTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetByIdAsync_ExistingId_ReturnsContact()
    {
        //Arrange
        var contactId = 1;
        var contact = new Contact { Id = 1, Name = "Test User 1", Phone = "99983-1617", Email = "testUser1@gmail.com", DddId = 11 };

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(contact)
        };

        var messageHandlerMock = new Mock<HttpMessageHandler>();
        var mockHandler = MockHttpClientFactory.CreateMockHttpClient(response, messageHandlerMock);
        var mockHttpClientFactory = MockHttpClientFactory.CreateMockFactory(mockHandler);

        var contactRepository = new ContactHttpRepository(mockHttpClientFactory);
        var dddRepository = new DirectDistanceDialingHttpRepository(mockHttpClientFactory);

        var contactService = new ContactService(contactRepository, dddRepository);

        // Act
        var result = await contactService.GetByIdAsync(contactId);

        // Assert
        messageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get && req.RequestUri.ToString().EndsWith($"/persistence-api/v1/contacts/{contactId}")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllAsync_ReturnsAllContacts()
    {
        //Arrange
        var contacts = new List<Contact>
        {
            new Contact { Id = 1, Name = "Test User 1", Phone = "99983-1617", Email = "testUser1@gmail.com", DddId = 11 },
            new Contact { Id = 2, Name = "Test User 2", Phone = "99983-1618", Email = "testUser2@gmail.com", DddId = 11 }
        };

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(contacts)
        };

        var messageHandlerMock = new Mock<HttpMessageHandler>();
        var mockHandler = MockHttpClientFactory.CreateMockHttpClient(response, messageHandlerMock);
        var mockHttpClientFactory = MockHttpClientFactory.CreateMockFactory(mockHandler);

        var contactRepository = new ContactHttpRepository(mockHttpClientFactory);
        var dddRepository = new DirectDistanceDialingHttpRepository(mockHttpClientFactory);

        var contactService = new ContactService(contactRepository, dddRepository);

        // Act
        var result = await contactService.GetAllAsync();

        // Assert
        messageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get && req.RequestUri.ToString().EndsWith("/persistence-api/v1/contacts")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllByDddAsync_ValidDddId_ReturnsContacts()
    {
        //Arrange
        var dddCode = 11;
        var contacts = new List<Contact>
        {
            new Contact { Id = 1, Name = "Test User 1", Phone = "99983-1617", Email = "testUser1@gmail.com", DddId = dddCode },
            new Contact { Id = 2, Name = "Test User 2", Phone = "99983-1618", Email = "testUser2@gmail.com", DddId = dddCode }
        };

        var ddd = new DirectDistanceDialing { Id = dddCode, Region = "São Paulo", CreatedOn = DateTime.Now };

        var contactsResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(contacts)
        };

        var dddResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(ddd)
        };

        var messageContactHandlerMock = new Mock<HttpMessageHandler>();
        var messageDddHandlerMock = new Mock<HttpMessageHandler>();

        var mockContactHandler = MockHttpClientFactory.CreateMockHttpClient(contactsResponse, messageContactHandlerMock);
        var mockContactHttpClientFactory = MockHttpClientFactory.CreateMockFactory(mockContactHandler);

        var mockDddHandler = MockHttpClientFactory.CreateMockHttpClient(dddResponse, messageDddHandlerMock);
        var mockDddHttpClientFactory = MockHttpClientFactory.CreateMockFactory(mockDddHandler);

        var contactRepository = new ContactHttpRepository(mockContactHttpClientFactory);
        var dddRepository = new DirectDistanceDialingHttpRepository(mockDddHttpClientFactory);

        var contactService = new ContactService(contactRepository, dddRepository);

        // Act
        var result = await contactService.GetAllByDddAsync(dddCode);

        // Assert
        messageContactHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get && req.RequestUri.ToString().EndsWith($"/persistence-api/v1/contacts/ddd-code/{dddCode}")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateAsync_ValidEntity_CallsRepositoryCreate()
    {
        // Arrange
        var contact = new Contact { Id = 1, Name = "Test User 1", Phone = "99983-1617", Email = "testUser1@gmail.com", DddId = 11 };

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Created,
            Content = JsonContent.Create(contact)
        };

        var messageHandlerMock = new Mock<HttpMessageHandler>();
        var mockHandler = MockHttpClientFactory.CreateMockHttpClient(response, messageHandlerMock);
        var mockHttpClientFactory = MockHttpClientFactory.CreateMockFactory(mockHandler);

        var contactRepository = new ContactHttpRepository(mockHttpClientFactory);
        var dddRepository = new DirectDistanceDialingHttpRepository(mockHttpClientFactory);

        var contactService = new ContactService(contactRepository, dddRepository);

        // Act
        await contactService.CreateAsync(contact);

        // Assert
        messageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post && req.RequestUri.ToString().EndsWith("/persistence-api/v1/contacts")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task EditAsync_ValidEntity_CallsRepositoryEdit()
    {
        // Arrange
        var contactId = 1;
        var contact = new Contact { Id = 1, Name = "Test User 1", Phone = "99983-1617", Email = "testUser1@gmail.com", DddId = 11 };

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(contact)
        };

        var messageHandlerMock = new Mock<HttpMessageHandler>();
        var mockHandler = MockHttpClientFactory.CreateMockHttpClient(response, messageHandlerMock);
        var mockHttpClientFactory = MockHttpClientFactory.CreateMockFactory(mockHandler);

        var contactRepository = new ContactHttpRepository(mockHttpClientFactory);
        var dddRepository = new DirectDistanceDialingHttpRepository(mockHttpClientFactory);

        var contactService = new ContactService(contactRepository, dddRepository);

        // Act
        await contactService.EditAsync(contact);

        // Assert
        messageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Put && req.RequestUri.ToString().EndsWith($"/persistence-api/v1/contacts/{contactId}")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteAsync_ValidEntity_CallsRepositoryDelete()
    {
        // Arrange
        var contactId = 1;
        var contact = new Contact { Id = 1, Name = "Test User 1", Phone = "99983-1617", Email = "testUser1@gmail.com", DddId = 11 };

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NoContent,
        };

        var messageHandlerMock = new Mock<HttpMessageHandler>();
        var mockHandler = MockHttpClientFactory.CreateMockHttpClient(response, messageHandlerMock);
        var mockHttpClientFactory = MockHttpClientFactory.CreateMockFactory(mockHandler);

        var contactRepository = new ContactHttpRepository(mockHttpClientFactory);
        var dddRepository = new DirectDistanceDialingHttpRepository(mockHttpClientFactory);

        var contactService = new ContactService(contactRepository, dddRepository);

        // Act
        await contactService.DeleteAsync(contact);

        // Assert
        messageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Delete && req.RequestUri.ToString().EndsWith($"/persistence-api/v1/contacts/{contactId}")),
            ItExpr.IsAny<CancellationToken>());
    }

}
