using Application.Service;
using Application.Service.Interface;
using Core.Entity;
using Core.Message.Command;
using Core.Message.Interface;
using Core.Repository.Interface;
using Infraestructure.Message;
using Infraestructure.Repository;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
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
        var mockMessagePublisher = new Mock<IMessagePublisher>();

        var contactRepository = new ContactHttpRepository(mockHttpClientFactory);
        var dddRepository = new DirectDistanceDialingHttpRepository(mockHttpClientFactory);        

        var contactService = new ContactService(contactRepository, dddRepository, mockMessagePublisher.Object);

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
        var mockMessagePublisher = new Mock<IMessagePublisher>();

        var contactRepository = new ContactHttpRepository(mockHttpClientFactory);
        var dddRepository = new DirectDistanceDialingHttpRepository(mockHttpClientFactory);

        var contactService = new ContactService(contactRepository, dddRepository, mockMessagePublisher.Object);

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
        var mockMessagePublisher = new Mock<IMessagePublisher>();

        var contactService = new ContactService(contactRepository, dddRepository, mockMessagePublisher.Object);

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
    public async Task CreateAsync_ValidCommand_PublishesCreateContactCommand()
    {
        // Arrange
        var services = ConfigureDependencyInjection();

        await using var provider = services.BuildServiceProvider();
        var harness = provider.GetRequiredService<ITestHarness>();
        var contactService = provider.GetRequiredService<IContactService>();
        await harness.Start();

        var command = new CreateContactCommand
        (
            "Test User 1",
            "99983-1617",
            "testUser1@gmail.com",
            11
        );

        // Act
        await contactService.CreateAsync(command);

        // Assert
        Assert.True(await harness.Published.Any<CreateContactCommand>());

        await harness.Stop();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task EditAsync_ValidCommand_PublishesEditContactCommand()
    {
        // Arrange
        var services = ConfigureDependencyInjection();

        await using var provider = services.BuildServiceProvider();
        var harness = provider.GetRequiredService<ITestHarness>();
        var contactService = provider.GetRequiredService<IContactService>();        
        await harness.Start();

        var command = new EditContactCommand
        (
            1,
            "Updated User",
            "11122-3344",
            "updated@example.com",
            11
        );

        // Act
        await contactService.EditAsync(command);

        // Assert
        Assert.True(await harness.Published.Any<EditContactCommand>());

        await harness.Stop();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteAsync_ValidCommand_PublishesDeleteContactCommand()
    {
        // Arrange
        var services = ConfigureDependencyInjection();

        await using var provider = services.BuildServiceProvider();
        var harness = provider.GetRequiredService<ITestHarness>();
        var contactService = provider.GetRequiredService<IContactService>();
        await harness.Start();

        var command = new DeleteContactCommand(1);        

        // Act
        await contactService.DeleteAsync(command);

        // Assert
        Assert.True(await harness.Published.Any<DeleteContactCommand>());

        await harness.Stop();
    }
    
    private ServiceCollection ConfigureDependencyInjection()
    {
        var services = new ServiceCollection();
        services.AddMassTransitTestHarness(x => { });
        services.AddScoped<IMessagePublisher, MessagePublisher>();
        services.AddScoped<IContactService, ContactService>();

        var mockContactHttpRepository = new Mock<IContactHttpRepository>();
        var mockDirectDistanceDialingHttpRepository = new Mock<IDirectDistanceDialingHttpRepository>();
        services.AddSingleton(mockContactHttpRepository.Object);
        services.AddSingleton(mockDirectDistanceDialingHttpRepository.Object);

        return services;
    }
}
