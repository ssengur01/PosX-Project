using System.Net.Http.Json;

namespace Reports.API.Services;

public class CustomersApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CustomersApiService> _logger;

    public CustomersApiService(HttpClient httpClient, ILogger<CustomersApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<CustomerDto>?> GetAllAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<CustomerDto>>("/api/v1/customers");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching customers from Customers.API");
            return null;
        }
    }
}

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
);
