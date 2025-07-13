using Application.Service;
using Core.Entity;
using Core.Repository.Interface;
using Moq;

namespace UnitTest.Application.Service;
public class ContactServiceTests
{
    private readonly Mock<IContactHttpRepository> _mockContactHttpRepository;
    private readonly Mock<IDirectDistanceDialingHttpRepository> _mockDddHttpRepository;
    private readonly ContactService _contactService;

    public ContactServiceTests()
    {
        _mockContactHttpRepository = new Mock<IContactHttpRepository>();
        _mockDddHttpRepository = new Mock<IDirectDistanceDialingHttpRepository>();
        _contactService = new ContactService(_mockContactHttpRepository.Object, _mockDddHttpRepository.Object);
    }

    [Fact]
    public async Task GetAllByDddAsync_ValidDddId_ReturnsContacts()
    {
        // Arrange
        int dddId = 11;
        var ddd = new DirectDistanceDialing { Id = dddId, Region = "São Paulo", CreatedOn = DateTime.Now  };
        var contacts = new List<Contact>
        {
            new() { Id = 1, Name = "Test User 1", Phone = "99983-1617", Email="testUser1@gmail.com", DddId= dddId },
            new() { Id = 2, Name = "Test User 2" , Phone= "99983-1618", Email="testUser2@gmail.com", DddId= dddId }
        };

        _mockDddHttpRepository.Setup(repo => repo.GetByIdAsync(dddId)).ReturnsAsync(ddd);
        _mockContactHttpRepository.Setup(repo => repo.GetAllByDddAsync(dddId)).ReturnsAsync(contacts);

        // Act
        var result = await _contactService.GetAllByDddAsync(dddId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllByDddAsync_InvalidDddId_ThrowsArgumentException()
    {
        // Arrange
        int dddId = 999;
        _mockDddHttpRepository.Setup(repo => repo.GetByIdAsync(dddId)).ReturnsAsync(value: null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _contactService.GetAllByDddAsync(dddId));
        Assert.Equal("Invalid Direct Distance Dialing Id", exception.Message);
    }
}
