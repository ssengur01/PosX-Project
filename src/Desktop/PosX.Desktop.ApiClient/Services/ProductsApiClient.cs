using System.Net.Http.Json;
using PosX.Desktop.ApiClient.Models;

namespace PosX.Desktop.ApiClient.Services;

public class ProductsApiClient
{
    private readonly HttpClient _httpClient;

    public ProductsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResult<ProductDto>?> GetProductsAsync(int page = 1, int pageSize = 20)
    {
        var response = await _httpClient.GetAsync($"/api/v1/products?page={page}&pageSize={pageSize}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();
        }
        return null;
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/api/v1/products/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ProductDto>();
        }
        return null;
    }

    public async Task<List<ProductDto>?> SearchProductsAsync(string searchTerm)
    {
        var response = await _httpClient.GetAsync($"/api/v1/products/search?searchTerm={Uri.EscapeDataString(searchTerm)}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        }
        return null;
    }
}
