using GymManagement.Application.DTOs;

namespace GymManagement.Application.Interfaces;

/// <summary>
/// Service interface for membership operations. (DIP)
/// </summary>
public interface IMembershipService
{
    Task<MembershipDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MembershipDto>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default);
    Task<MembershipDto> CreateAsync(CreateMembershipDto dto, CancellationToken cancellationToken = default);
    Task<MembershipDto?> RenewAsync(int id, RenewMembershipDto dto, CancellationToken cancellationToken = default);
    Task<MembershipDto?> SuspendAsync(int id, CancellationToken cancellationToken = default);
    Task<MembershipDto?> CancelAsync(int id, CancellationToken cancellationToken = default);
}
