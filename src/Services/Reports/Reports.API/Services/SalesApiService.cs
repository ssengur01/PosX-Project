using System.Net.Http.Json;

namespace Reports.API.Services;

public class SalesApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SalesApiService> _logger;

    public SalesApiService(HttpClient httpClient, ILogger<SalesApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<SaleDto>?> GetSalesAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var url = "/api/v1/sales";
            if (startDate.HasValue && endDate.HasValue)
            {
                url += $"?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
            }

            return await _httpClient.GetFromJsonAsync<List<SaleDto>>(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching sales from Sales.API");
            return null;
        }
    }
}

public record SaleDto(
    Guid Id,
    string SaleNumber,
    DateTime SaleDate,
    Guid EmployeeId,
    Guid? CustomerId,
    decimal Subtotal,
    decimal Tax,
    decimal Total,
    string Status
);
