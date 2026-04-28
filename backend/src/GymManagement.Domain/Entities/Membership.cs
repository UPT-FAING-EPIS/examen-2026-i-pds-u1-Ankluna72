using GymManagement.Domain.Enums;

namespace GymManagement.Domain.Entities;

/// <summary>
/// Represents a gym membership. Domain entity with business rules encapsulated.
/// </summary>
public class Membership
{
    public int Id { get; private set; }
    public int ClientId { get; private set; }
    public string PlanName { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public MembershipStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public Client? Client { get; private set; }

    private Membership() { }

    public static Membership Create(int clientId, string planName, decimal price, DateTime startDate, DateTime endDate)
    {
        if (string.IsNullOrWhiteSpace(planName)) throw new ArgumentException("Plan name is required.");
        if (price <= 0) throw new ArgumentException("Price must be greater than zero.");
        if (endDate <= startDate) throw new ArgumentException("End date must be after start date.");

        return new Membership
        {
            ClientId = clientId,
            PlanName = planName,
            Price = price,
            StartDate = startDate,
            EndDate = endDate,
            Status = MembershipStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Renews the membership extending the end date.
    /// </summary>
    public void Renew(DateTime newEndDate)
    {
        if (newEndDate <= EndDate) throw new ArgumentException("New end date must be after current end date.");
        EndDate = newEndDate;
        Status = MembershipStatus.Active;
    }

    public void Suspend() => Status = MembershipStatus.Suspended;
    public void Cancel() => Status = MembershipStatus.Cancelled;

    /// <summary>
    /// Checks and updates status based on current date.
    /// </summary>
    public void CheckExpiry()
    {
        if (Status == MembershipStatus.Active && DateTime.UtcNow > EndDate)
            Status = MembershipStatus.Expired;
    }

    public bool IsValid => Status == MembershipStatus.Active && DateTime.UtcNow <= EndDate;
}
