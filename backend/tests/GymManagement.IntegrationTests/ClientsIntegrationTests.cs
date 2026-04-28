using System.Net;
using System.Net.Http.Json;
using GymManagement.Application.DTOs;
using Xunit;

namespace GymManagement.IntegrationTests;

public class ClientsIntegrationTests : IClassFixture<GymWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ClientsIntegrationTests(GymWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostClient_ValidData_Returns201Created()
    {
        var dto = new CreateClientDto("Integration", "Test", $"it_{Guid.NewGuid()}@test.com", "999-000-111", new DateTime(1995, 3, 15));
        var response = await _client.PostAsJsonAsync("/api/clients", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<ClientDto>();
        Assert.NotNull(created);
        Assert.Equal("Integration", created!.FirstName);
    }

    [Fact]
    public async Task GetClient_NonExisting_Returns404()
    {
        var response = await _client.GetAsync("/api/clients/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostClient_DuplicateEmail_Returns400()
    {
        var email = $"dup_{Guid.NewGuid()}@test.com";
        var dto = new CreateClientDto("Dup", "User", email, "999", new DateTime(1990, 1, 1));

        await _client.PostAsJsonAsync("/api/clients", dto);
        var secondResponse = await _client.PostAsJsonAsync("/api/clients", dto);

        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }
}
