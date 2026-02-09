namespace Customers.Application.DTOs;

public record CustomerDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Phone,
    string? Email,
    string? Address,
    int LoyaltyPoints,
    DateTime JoinedDate,
    DateTime? LastPurchaseDate,
    bool IsActive);

public record CreateCustomerDto(
    string FirstName,
    string LastName,
    string Phone,
    string? Email,
    string? Address);
