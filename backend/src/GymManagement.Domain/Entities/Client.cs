namespace GymManagement.Domain.Entities;

/// <summary>
/// Represents a gym client. (SRP - Entity only holds data and domain logic)
/// </summary>
public class Client
{
    public int Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public DateTime BirthDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public ICollection<Membership> Memberships { get; private set; } = new List<Membership>();
    public ICollection<WorkoutSession> WorkoutSessions { get; private set; } = new List<WorkoutSession>();

    // EF Core constructor
    private Client() { }

    /// <summary>
    /// Factory method — ensures valid Client creation (OCP / DIP)
    /// </summary>
    public static Client Create(string firstName, string lastName, string email, string phone, DateTime birthDate)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.");
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.");

        return new Client
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            BirthDate = birthDate,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public void Update(string firstName, string lastName, string phone)
    {
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
    }

    public void Deactivate() => IsActive = false;

    public string FullName => $"{FirstName} {LastName}";
}
