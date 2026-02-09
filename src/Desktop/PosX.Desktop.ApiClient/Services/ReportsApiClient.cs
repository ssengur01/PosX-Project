using System.Net.Http.Json;
using PosX.Desktop.ApiClient.Models;

namespace PosX.Desktop.ApiClient.Services;

public class ReportsApiClient
{
    private readonly HttpClient _httpClient;

    public ReportsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DashboardKpiDto?> GetDashboardKpisAsync()
    {
        var response = await _httpClient.GetAsync("/api/v1/dashboard/kpis");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<DashboardKpiDto>();
        }
        return null;
    }

    public async Task<List<SalesReportItemDto>?> GetDailySalesReportAsync(DateTime startDate, DateTime endDate)
    {
        var response = await _httpClient.GetAsync($"/api/v1/reports/sales/daily?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<SalesReportItemDto>>();
        }
        return null;
    }

    public async Task<List<SalesByEmployeeItemDto>?> GetSalesByEmployeeReportAsync(DateTime startDate, DateTime endDate)
    {
        var response = await _httpClient.GetAsync($"/api/v1/reports/sales/by-employee?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<SalesByEmployeeItemDto>>();
        }
        return null;
    }
}
