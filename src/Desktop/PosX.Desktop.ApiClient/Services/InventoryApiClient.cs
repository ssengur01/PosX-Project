using System.Net.Http.Json;
using PosX.Desktop.ApiClient.Models;

namespace PosX.Desktop.ApiClient.Services;

public class InventoryApiClient
{
    private readonly HttpClient _httpClient;

    public InventoryApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<InventoryItemDto>?> GetAllStockLevelsAsync()
    {
        var response = await _httpClient.GetAsync("/api/v1/inventory");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<InventoryItemDto>>();
        }
        return null;
    }

    public async Task<InventoryItemDto?> GetStockByProductIdAsync(Guid productId)
    {
        var response = await _httpClient.GetAsync($"/api/v1/inventory?productId={productId}");
        if (response.IsSuccessStatusCode)
        {
            var items = await response.Content.ReadFromJsonAsync<List<InventoryItemDto>>();
            return items?.FirstOrDefault();
        }
        return null;
    }

    public async Task<bool> AdjustStockAsync(AdjustStockRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/v1/inventory/adjust", request);
        return response.IsSuccessStatusCode;
    }
}
