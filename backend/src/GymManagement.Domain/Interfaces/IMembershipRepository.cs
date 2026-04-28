using GymManagement.Domain.Entities;

namespace GymManagement.Domain.Interfaces;

/// <summary>
/// Repository contract for Membership aggregate. (ISP - specific interface)
/// </summary>
public interface IMembershipRepository
{
    Task<Membership?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Membership>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default);
    Task AddAsync(Membership membership, CancellationToken cancellationToken = default);
    Task UpdateAsync(Membership membership, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
