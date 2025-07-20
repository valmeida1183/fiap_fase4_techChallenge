using Application.Service;
using Core.Entity;
using Core.Message.Command;
using Core.Message.Interface;
using Core.Repository.Interface;
using Moq;

namespace UnitTest.Application.Service;
public class ContactServiceTests
{
    private readonly Mock<IContactHttpRepository> _mockContactHttpRepository;
    private readonly Mock<IDirectDistanceDialingHttpRepository> _mockDddHttpRepository;
    private readonly Mock<IMessagePublisher> _mockMessagePublisher;
    private readonly ContactService _contactService;

    public ContactServiceTests()
    {
        _mockContactHttpRepository = new Mock<IContactHttpRepository>();
        _mockDddHttpRepository = new Mock<IDirectDistanceDialingHttpRepository>();
        _mockMessagePublisher = new Mock<IMessagePublisher>();
        _contactService = new ContactService(
            _mockContactHttpRepository.Object,
            _mockDddHttpRepository.Object,
            _mockMessagePublisher.Object
        );
    }

    [Fact]
    public async Task GetAllByDddAsync_ValidDddId_ReturnsContacts()
    {
        // Arrange
        int dddId = 11;
        var ddd = new DirectDistanceDialing { Id = dddId, Region = "São Paulo", CreatedOn = DateTime.Now };
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

    [Fact]
    public async Task CreateAsync_ValidCommand_CallsMessagePublish()
    {
        // Arrange
        var command = new CreateContactCommand("Test User 1", "99983-1617", "testUser1@gmail.com", 11);

        _mockMessagePublisher.Setup(mp => mp.Publish(command, default))
            .Returns(Task.CompletedTask);

        // Act
        await _contactService.CreateAsync(command);

        // Assert
        _mockMessagePublisher.Verify(mp => mp.Publish(command, default), Times.Once);
    }

    [Fact]
    public async Task EditAsync_ValidCommand_CallsMessagePublish()
    {
        // Arrange
        var command = new EditContactCommand(1, "Test User 1", "99983-1617", "testUser1@gmail.com", 11);
        
        _mockMessagePublisher.Setup(mp => mp.Publish(command, default))
           .Returns(Task.CompletedTask);

        // Act
        await _contactService.EditAsync(command);

        // Assert
        _mockMessagePublisher.Verify(mp => mp.Publish(command, default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ValidCommand_CallsMessagePublish()
    {
        // Arrange
        var command = new DeleteContactCommand(1);

        _mockMessagePublisher.Setup(mp => mp.Publish(command, default))
            .Returns(Task.CompletedTask);


        // Act
        await _contactService.DeleteAsync(command);

        // Assert
        _mockMessagePublisher.Verify(mp => mp.Publish(command, default), Times.Once);
    }
}
