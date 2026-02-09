namespace PosX.Desktop.ApiClient.Models;

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
    bool IsActive
)
{
    public string FullName => $"{FirstName} {LastName}";
    public string DisplayName => $"{FullName} ({Phone})";
}

public record CreateCustomerRequest(
    string FirstName,
    string LastName,
    string Phone,
    string? Email,
    string? Address
);

public record UpdateCustomerRequest(
    Guid Id,
    string FirstName,
    string LastName,
    string Phone,
    string? Email,
    string? Address
);

public record AddLoyaltyPointsRequest(Guid CustomerId, int Points);
public record RedeemPointsRequest(Guid CustomerId, int Points);
