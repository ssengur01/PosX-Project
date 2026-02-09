using System.Net.Http.Json;

namespace Reports.API.Services;

public class EmployeesApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmployeesApiService> _logger;

    public EmployeesApiService(HttpClient httpClient, ILogger<EmployeesApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<EmployeeDto>?> GetAllAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<EmployeeDto>>("/api/v1/employees");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching employees from Employees.API");
            return null;
        }
    }
}

public record EmployeeDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    Guid RoleId,
    decimal SalaryAmount,
    string SalaryCurrency,
    decimal? CommissionRate,
    decimal? CommissionMinimumSales,
    DateTime HireDate,
    string Status
);
