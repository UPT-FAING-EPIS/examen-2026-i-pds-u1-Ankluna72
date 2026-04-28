using GymManagement.Application.DTOs;
using GymManagement.Application.Interfaces;
using GymManagement.Domain.Entities;
using GymManagement.Domain.Interfaces;

namespace GymManagement.Application.Services;

/// <summary>
/// Client business logic service. (SRP - Only handles client business logic)
/// (DIP - Depends on IClientRepository abstraction, not concrete implementation)
/// </summary>
public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    /// <inheritdoc/>
    public async Task<ClientDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = await _clientRepository.GetByIdAsync(id, cancellationToken);
        return client is null ? null : MapToDto(client);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ClientDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var clients = await _clientRepository.GetAllAsync(cancellationToken);
        return clients.Select(MapToDto);
    }

    /// <inheritdoc/>
    public async Task<ClientDto> CreateAsync(CreateClientDto dto, CancellationToken cancellationToken = default)
    {
        var emailExists = await _clientRepository.ExistsByEmailAsync(dto.Email, cancellationToken);
        if (emailExists) throw new InvalidOperationException($"A client with email '{dto.Email}' already exists.");

        var client = Client.Create(dto.FirstName, dto.LastName, dto.Email, dto.Phone, dto.BirthDate);
        await _clientRepository.AddAsync(client, cancellationToken);
        await _clientRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(client);
    }

    /// <inheritdoc/>
    public async Task<ClientDto?> UpdateAsync(int id, UpdateClientDto dto, CancellationToken cancellationToken = default)
    {
        var client = await _clientRepository.GetByIdAsync(id, cancellationToken);
        if (client is null) return null;

        client.Update(dto.FirstName, dto.LastName, dto.Phone);
        await _clientRepository.UpdateAsync(client, cancellationToken);
        await _clientRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(client);
    }

    private static ClientDto MapToDto(Client c) => new(
        c.Id, c.FirstName, c.LastName, c.FullName,
        c.Email, c.Phone, c.BirthDate, c.CreatedAt, c.IsActive
    );
}
