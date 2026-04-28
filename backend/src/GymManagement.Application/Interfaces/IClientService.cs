using GymManagement.Application.DTOs;

namespace GymManagement.Application.Interfaces;

/// <summary>
/// Service interface for client operations. (DIP - high-level modules depend on abstractions)
/// </summary>
public interface IClientService
{
    Task<ClientDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ClientDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ClientDto> CreateAsync(CreateClientDto dto, CancellationToken cancellationToken = default);
    Task<ClientDto?> UpdateAsync(int id, UpdateClientDto dto, CancellationToken cancellationToken = default);
}
