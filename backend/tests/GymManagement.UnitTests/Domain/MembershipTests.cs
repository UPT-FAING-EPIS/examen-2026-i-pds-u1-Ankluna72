using GymManagement.Domain.Entities;
using GymManagement.Domain.Enums;
using Xunit;

namespace GymManagement.UnitTests.Domain;

public class MembershipTests
{
    [Fact]
    public void Create_ValidData_ShouldReturnActiveMembership()
    {
        var start = DateTime.Today;
        var end = DateTime.Today.AddMonths(1);
        var membership = Membership.Create(1, "Monthly Plan", 50m, start, end);

        Assert.Equal(MembershipStatus.Active, membership.Status);
        Assert.Equal("Monthly Plan", membership.PlanName);
        Assert.Equal(50m, membership.Price);
        Assert.True(membership.IsValid);
    }

    [Fact]
    public void Create_PriceZero_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            Membership.Create(1, "Plan", 0m, DateTime.Today, DateTime.Today.AddMonths(1)));
    }

    [Fact]
    public void Create_EndBeforeStart_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            Membership.Create(1, "Plan", 50m, DateTime.Today, DateTime.Today.AddDays(-1)));
    }

    [Fact]
    public void Renew_ValidDate_ShouldExtendEndDate()
    {
        var membership = Membership.Create(1, "Plan", 50m, DateTime.Today, DateTime.Today.AddMonths(1));
        var newEnd = DateTime.Today.AddMonths(2);
        membership.Renew(newEnd);

        Assert.Equal(newEnd, membership.EndDate);
        Assert.Equal(MembershipStatus.Active, membership.Status);
    }

    [Fact]
    public void Renew_EarlierDate_ShouldThrowArgumentException()
    {
        var membership = Membership.Create(1, "Plan", 50m, DateTime.Today, DateTime.Today.AddMonths(1));
        Assert.Throws<ArgumentException>(() => membership.Renew(DateTime.Today));
    }

    [Fact]
    public void Suspend_ShouldChangStatusToSuspended()
    {
        var membership = Membership.Create(1, "Plan", 50m, DateTime.Today, DateTime.Today.AddMonths(1));
        membership.Suspend();
        Assert.Equal(MembershipStatus.Suspended, membership.Status);
    }

    [Fact]
    public void Cancel_ShouldChangeStatusToCancelled()
    {
        var membership = Membership.Create(1, "Plan", 50m, DateTime.Today, DateTime.Today.AddMonths(1));
        membership.Cancel();
        Assert.Equal(MembershipStatus.Cancelled, membership.Status);
    }
}
