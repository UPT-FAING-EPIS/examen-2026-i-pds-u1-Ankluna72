using GymManagement.Application.DTOs;
using GymManagement.Application.Interfaces;
using GymManagement.Domain.Entities;
using GymManagement.Domain.Interfaces;

namespace GymManagement.Application.Services;

/// <summary>
/// Membership business logic service. (SRP - Only handles membership business logic)
/// (DIP - Depends on abstractions, not concrete repos)
/// </summary>
public class MembershipService : IMembershipService
{
    private readonly IMembershipRepository _membershipRepository;
    private readonly IClientRepository _clientRepository;

    public MembershipService(IMembershipRepository membershipRepository, IClientRepository clientRepository)
    {
        _membershipRepository = membershipRepository;
        _clientRepository = clientRepository;
    }

    /// <inheritdoc/>
    public async Task<MembershipDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var membership = await _membershipRepository.GetByIdAsync(id, cancellationToken);
        if (membership is null) return null;
        membership.CheckExpiry();
        return MapToDto(membership);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<MembershipDto>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var memberships = await _membershipRepository.GetByClientIdAsync(clientId, cancellationToken);
        foreach (var m in memberships) m.CheckExpiry();
        return memberships.Select(MapToDto);
    }

    /// <inheritdoc/>
    public async Task<MembershipDto> CreateAsync(CreateMembershipDto dto, CancellationToken cancellationToken = default)
    {
        var client = await _clientRepository.GetByIdAsync(dto.ClientId, cancellationToken)
            ?? throw new InvalidOperationException($"Client with id {dto.ClientId} not found.");

        var membership = Membership.Create(dto.ClientId, dto.PlanName, dto.Price, dto.StartDate, dto.EndDate);
        await _membershipRepository.AddAsync(membership, cancellationToken);
        await _membershipRepository.SaveChangesAsync(cancellationToken);

        // Reload with client navigation for DTO
        var saved = await _membershipRepository.GetByIdAsync(membership.Id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to retrieve saved membership.");

        return MapToDto(saved, client.FullName);
    }

    /// <inheritdoc/>
    public async Task<MembershipDto?> RenewAsync(int id, RenewMembershipDto dto, CancellationToken cancellationToken = default)
    {
        var membership = await _membershipRepository.GetByIdAsync(id, cancellationToken);
        if (membership is null) return null;

        membership.Renew(dto.NewEndDate);
        await _membershipRepository.UpdateAsync(membership, cancellationToken);
        await _membershipRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(membership);
    }

    /// <inheritdoc/>
    public async Task<MembershipDto?> SuspendAsync(int id, CancellationToken cancellationToken = default)
    {
        var membership = await _membershipRepository.GetByIdAsync(id, cancellationToken);
        if (membership is null) return null;

        membership.Suspend();
        await _membershipRepository.UpdateAsync(membership, cancellationToken);
        await _membershipRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(membership);
    }

    /// <inheritdoc/>
    public async Task<MembershipDto?> CancelAsync(int id, CancellationToken cancellationToken = default)
    {
        var membership = await _membershipRepository.GetByIdAsync(id, cancellationToken);
        if (membership is null) return null;

        membership.Cancel();
        await _membershipRepository.UpdateAsync(membership, cancellationToken);
        await _membershipRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(membership);
    }

    private static MembershipDto MapToDto(Membership m, string? clientFullName = null) => new(
        m.Id, m.ClientId, clientFullName ?? m.Client?.FullName ?? string.Empty,
        m.PlanName, m.Price, m.StartDate, m.EndDate,
        m.Status, m.Status.ToString(), m.IsValid, m.CreatedAt
    );
}
