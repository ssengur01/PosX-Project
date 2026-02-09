using System.Net.Http.Json;
using PosX.Desktop.ApiClient.Models;

namespace PosX.Desktop.ApiClient.Services;

public class SalesApiClient
{
    private readonly HttpClient _httpClient;

    public SalesApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SaleResponse?> CreateSaleAsync(CreateSaleRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/v1/sales", request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<SaleResponse>();
        }
        return null;
    }

    public async Task<SaleResponse?> GetSaleByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/api/v1/sales/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<SaleResponse>();
        }
        return null;
    }

    public async Task<List<SaleResponse>?> GetSalesAsync(DateTime? startDate = null, DateTime? endDate = null, Guid? employeeId = null)
    {
        var queryParams = new List<string>();
        if (startDate.HasValue)
            queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
        if (endDate.HasValue)
            queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
        if (employeeId.HasValue)
            queryParams.Add($"employeeId={employeeId.Value}");

        var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        var response = await _httpClient.GetAsync($"/api/v1/sales{query}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<SaleResponse>>();
        }
        return null;
    }

    public async Task<RefundResponse?> ProcessRefundAsync(Guid saleId, ProcessRefundRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/v1/sales/{saleId}/refund", request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<RefundResponse>();
        }
        return null;
    }

    public async Task<OpenShiftResponse?> OpenShiftAsync(OpenShiftRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/v1/shifts/open", request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<OpenShiftResponse>();
        }
        return null;
    }

    public async Task<bool> CloseShiftAsync(Guid shiftId, CloseShiftRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/v1/shifts/{shiftId}/close", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AddCashMovementAsync(Guid shiftId, AddCashMovementRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/v1/shifts/{shiftId}/cash-movement", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<ShiftResponse?> GetOpenShiftAsync(Guid employeeId)
    {
        var response = await _httpClient.GetAsync($"/api/v1/shifts/open/employee/{employeeId}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ShiftResponse>();
        }
        return null;
    }
}
