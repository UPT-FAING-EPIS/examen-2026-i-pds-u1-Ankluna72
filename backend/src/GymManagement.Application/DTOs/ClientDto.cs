namespace GymManagement.Application.DTOs;

/// <summary>
/// DTO for returning client information.
/// </summary>
public record ClientDto(
    int Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string Phone,
    DateTime BirthDate,
    DateTime CreatedAt,
    bool IsActive
);

/// <summary>
/// DTO for creating a new client.
/// </summary>
public record CreateClientDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    DateTime BirthDate
);

/// <summary>
/// DTO for updating client information.
/// </summary>
public record UpdateClientDto(
    string FirstName,
    string LastName,
    string Phone
);
