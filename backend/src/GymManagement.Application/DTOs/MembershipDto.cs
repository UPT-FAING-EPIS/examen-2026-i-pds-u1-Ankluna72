using GymManagement.Domain.Enums;

namespace GymManagement.Application.DTOs;

/// <summary>
/// DTO for returning membership information.
/// </summary>
public record MembershipDto(
    int Id,
    int ClientId,
    string ClientFullName,
    string PlanName,
    decimal Price,
    DateTime StartDate,
    DateTime EndDate,
    MembershipStatus Status,
    string StatusName,
    bool IsValid,
    DateTime CreatedAt
);

/// <summary>
/// DTO for creating a new membership.
/// </summary>
public record CreateMembershipDto(
    int ClientId,
    string PlanName,
    decimal Price,
    DateTime StartDate,
    DateTime EndDate
);

/// <summary>
/// DTO for renewing a membership.
/// </summary>
public record RenewMembershipDto(DateTime NewEndDate);
