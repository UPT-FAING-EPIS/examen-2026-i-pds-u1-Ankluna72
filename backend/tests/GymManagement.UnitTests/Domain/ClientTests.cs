using GymManagement.Domain.Entities;
using GymManagement.Domain.Enums;
using Xunit;

namespace GymManagement.UnitTests.Domain;

public class ClientTests
{
    [Fact]
    public void Create_ValidData_ShouldReturnClient()
    {
        // Arrange & Act
        var client = Client.Create("John", "Doe", "john@example.com", "999-123-456", new DateTime(1990, 1, 1));

        // Assert
        Assert.Equal("John", client.FirstName);
        Assert.Equal("Doe", client.LastName);
        Assert.Equal("john@example.com", client.Email);
        Assert.True(client.IsActive);
        Assert.Equal("John Doe", client.FullName);
    }

    [Theory]
    [InlineData("", "Doe", "john@example.com")]
    [InlineData("John", "", "john@example.com")]
    [InlineData("John", "Doe", "")]
    public void Create_InvalidData_ShouldThrowArgumentException(string firstName, string lastName, string email)
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Client.Create(firstName, lastName, email, "999-123-456", DateTime.Now));
    }

    [Fact]
    public void Update_ValidData_ShouldUpdateFields()
    {
        var client = Client.Create("John", "Doe", "john@example.com", "999-123-456", new DateTime(1990, 1, 1));
        client.Update("Jane", "Smith", "888-777-666");

        Assert.Equal("Jane", client.FirstName);
        Assert.Equal("Smith", client.LastName);
        Assert.Equal("888-777-666", client.Phone);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        var client = Client.Create("John", "Doe", "john@example.com", "999-123-456", new DateTime(1990, 1, 1));
        client.Deactivate();
        Assert.False(client.IsActive);
    }
}
