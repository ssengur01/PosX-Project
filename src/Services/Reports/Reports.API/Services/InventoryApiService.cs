using System.Net.Http.Json;

namespace Reports.API.Services;

public class InventoryApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InventoryApiService> _logger;

    public InventoryApiService(HttpClient httpClient, ILogger<InventoryApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<InventoryItemDto>?> GetAllStockLevelsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<InventoryItemDto>>("/api/v1/inventory");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching inventory from Inventory.API");
            return null;
        }
    }
}

public record InventoryItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    int ReorderLevel,
    DateTime LastUpdated
);
