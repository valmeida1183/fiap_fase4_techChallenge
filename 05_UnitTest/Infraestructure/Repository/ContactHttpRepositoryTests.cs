using Core.Entity;
using Infraestructure.Repository;
using System.Net;
using System.Net.Http.Json;
using UnitTest.Infraestructure.Repository.Base;

namespace UnitTest.Infraestructure.Repository;
public class ContactHttpRepositoryTests : BaseHttpRepositoryTests
{
    [Fact]
    public async Task GetAllByDddAsync_ValidDddId_ReturnsContacts()
    {

        // Arrange
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

        var httpClient = CreateMockHttpClient(response);
        var mockFactory = CreateMockFactory(httpClient);

        var httpRepository = new ContactHttpRepository(mockFactory);

        // Act
        var result = await httpRepository.GetAllByDddAsync(11);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Test User 1", result[0].Name);
        Assert.Equal("Test User 2", result[1].Name);
    }

    [Fact]
    public async Task ResilienceTest_ThrowsException_WhenFails()
    {
        // Arrange
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError
        };

        var httpClient = CreateMockHttpClient(response);
        var mockFactory = CreateMockFactory(httpClient);

        var repo = new ContactHttpRepository(mockFactory);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => repo.ResilienceTest(true));
    }
}
