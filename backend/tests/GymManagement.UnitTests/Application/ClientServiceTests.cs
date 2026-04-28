using GymManagement.Application.DTOs;
using GymManagement.Application.Services;
using GymManagement.Domain.Entities;
using GymManagement.Domain.Interfaces;
using Moq;
using Xunit;

namespace GymManagement.UnitTests.Application;

public class ClientServiceTests
{
    private readonly Mock<IClientRepository> _clientRepoMock;
    private readonly ClientService _clientService;

    public ClientServiceTests()
    {
        _clientRepoMock = new Mock<IClientRepository>();
        _clientService = new ClientService(_clientRepoMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingClient_ShouldReturnClientDto()
    {
        // Arrange
        var client = Client.Create("John", "Doe", "john@example.com", "999", new DateTime(1990, 1, 1));
        _clientRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(client);

        // Act
        var result = await _clientService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingClient_ShouldReturnNull()
    {
        _clientRepoMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((Client?)null);
        var result = await _clientService.GetByIdAsync(99);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_DuplicateEmail_ShouldThrowInvalidOperationException()
    {
        _clientRepoMock.Setup(r => r.ExistsByEmailAsync("john@example.com", default)).ReturnsAsync(true);

        var dto = new CreateClientDto("John", "Doe", "john@example.com", "999", DateTime.Today);
        await Assert.ThrowsAsync<InvalidOperationException>(() => _clientService.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ValidData_ShouldCreateAndReturnClient()
    {
        _clientRepoMock.Setup(r => r.ExistsByEmailAsync("new@example.com", default)).ReturnsAsync(false);
        _clientRepoMock.Setup(r => r.AddAsync(It.IsAny<Client>(), default)).Returns(Task.CompletedTask);
        _clientRepoMock.Setup(r => r.SaveChangesAsync(default)).ReturnsAsync(1);

        var dto = new CreateClientDto("Jane", "Smith", "new@example.com", "888", new DateTime(1995, 5, 10));
        var result = await _clientService.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Jane", result.FirstName);
        _clientRepoMock.Verify(r => r.AddAsync(It.IsAny<Client>(), default), Times.Once);
        _clientRepoMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }
}
