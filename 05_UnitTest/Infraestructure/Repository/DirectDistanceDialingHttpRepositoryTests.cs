using Core.Entity;
using Infraestructure.Repository;
using System.Net;
using System.Net.Http.Json;
using UnitTest.Infraestructure.Repository.Base;

namespace UnitTest.Infraestructure.Repository;
public class DirectDistanceDialingHttpRepositoryTests : BaseHttpRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsDirectDistanceDialings()
    {
        // Arrange
        var ddds = new List<DirectDistanceDialing>
        {
            new DirectDistanceDialing { Id = 11, Region = "São Paulo", CreatedOn = DateTime.Now },
            new DirectDistanceDialing { Id = 12, Region = "São Paulo", CreatedOn = DateTime.Now },
            new DirectDistanceDialing { Id = 13, Region = "São Paulo", CreatedOn = DateTime.Now }
        };

        var response = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = JsonContent.Create(ddds)
        };

        var httpClient = CreateMockHttpClient(response);
        var mockFactory = CreateMockFactory(httpClient);
        var httpRepository = new DirectDistanceDialingHttpRepository(mockFactory);

        // Act
        var result = await httpRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, c => c.Id == 11);
        Assert.Contains(result, c => c.Id == 12);
        Assert.Contains(result, c => c.Id == 13);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsDirectDistanceDialing()
    {
        // Arrange
        var ddd = new DirectDistanceDialing { Id = 11, Region = "São Paulo", CreatedOn = DateTime.Now };

        var response = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = JsonContent.Create(ddd)
        };

        var httpClient = CreateMockHttpClient(response);
        var mockFactory = CreateMockFactory(httpClient);
        var httpRepository = new DirectDistanceDialingHttpRepository(mockFactory);

        // Act
        var result = await httpRepository.GetByIdAsync(11);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(11, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var response = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.NoContent,
        };

        var httpClient = CreateMockHttpClient(response);
        var mockFactory = CreateMockFactory(httpClient);
        var httpRepository = new DirectDistanceDialingHttpRepository(mockFactory);

        // Act
        var result = await httpRepository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ValidEntity_AddsEntity()
    {
        // Arrange
        var ddd = new DirectDistanceDialing { Id = 11, Region = "São Paulo", CreatedOn = DateTime.Now };

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Created,
            Content = JsonContent.Create(ddd)
        };

        var httpClient = CreateMockHttpClient(response);
        var mockFactory = CreateMockFactory(httpClient);
        var httpRepository = new DirectDistanceDialingHttpRepository(mockFactory);

        // Act
        await httpRepository.CreateAsync(ddd);

        // Assert - no exception means success
    }

    [Fact]
    public async Task EditAsync_ExistingEntity_UpdatesEntity()
    {
        // Arrange
        var ddd = new DirectDistanceDialing { Id = 11, Region = "São Paulo", CreatedOn = DateTime.Now };

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(ddd)
        };

        var httpClient = CreateMockHttpClient(response);
        var mockFactory = CreateMockFactory(httpClient);
        var httpRepository = new DirectDistanceDialingHttpRepository(mockFactory);

        // Act
        await httpRepository.EditAsync(ddd);

        // Assert - no exception means success
    }

    [Fact]
    public async Task DeleteAsync_ExistingEntity_RemovesEntity()
    {
        // Arrange
        var ddd = new DirectDistanceDialing { Id = 11, Region = "São Paulo", CreatedOn = DateTime.Now };

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NoContent,
        };

        var httpClient = CreateMockHttpClient(response);
        var mockFactory = CreateMockFactory(httpClient);
        var httpRepository = new DirectDistanceDialingHttpRepository(mockFactory);

        // Act
        await httpRepository.DeleteAsync(ddd);

        // Assert - no exception means success
    }
}
