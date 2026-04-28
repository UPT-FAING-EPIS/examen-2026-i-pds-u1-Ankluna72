using GymManagement.Domain.Entities;

namespace GymManagement.Domain.Interfaces;

/// <summary>
/// Repository contract for Client aggregate. (ISP - specific interface)
/// </summary>
public interface IClientRepository
{
    Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Client>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(Client client, CancellationToken cancellationToken = default);
    Task UpdateAsync(Client client, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
