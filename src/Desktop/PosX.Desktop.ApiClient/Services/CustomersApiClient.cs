using System.Net.Http.Json;
using PosX.Desktop.ApiClient.Models;

namespace PosX.Desktop.ApiClient.Services;

public class CustomersApiClient
{
    private readonly HttpClient _httpClient;

    public CustomersApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CustomerDto>?> GetAllAsync(string? searchTerm = null)
    {
        var url = string.IsNullOrWhiteSpace(searchTerm)
            ? "/api/v1/customers"
            : $"/api/v1/customers?searchTerm={Uri.EscapeDataString(searchTerm)}";

        var response = await _httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<CustomerDto>>();
        }
        return null;
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/api/v1/customers/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CustomerDto>();
        }
        return null;
    }

    public async Task<CustomerDto?> GetByPhoneAsync(string phone)
    {
        var response = await _httpClient.GetAsync($"/api/v1/customers/phone/{Uri.EscapeDataString(phone)}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CustomerDto>();
        }
        return null;
    }

    public async Task<CustomerDto?> CreateAsync(CreateCustomerRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/v1/customers", request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CustomerDto>();
        }
        return null;
    }

    public async Task<CustomerDto?> UpdateAsync(Guid id, UpdateCustomerRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/v1/customers/{id}", request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CustomerDto>();
        }
        return null;
    }

    public async Task<bool> AddLoyaltyPointsAsync(Guid customerId, int points)
    {
        var request = new AddLoyaltyPointsRequest(customerId, points);
        var response = await _httpClient.PostAsJsonAsync($"/api/v1/customers/{customerId}/loyalty-points", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<CustomerDto?> RedeemPointsAsync(Guid customerId, int points)
    {
        var request = new RedeemPointsRequest(customerId, points);
        var response = await _httpClient.PostAsJsonAsync($"/api/v1/customers/{customerId}/redeem-points", request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CustomerDto>();
        }
        return null;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"/api/v1/customers/{id}");
        return response.IsSuccessStatusCode;
    }
}
