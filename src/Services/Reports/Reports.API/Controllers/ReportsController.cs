using Microsoft.AspNetCore.Mvc;
using Reports.API.Models;
using Reports.API.Services;

namespace Reports.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly SalesApiService _salesService;
    private readonly EmployeesApiService _employeesService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(
        SalesApiService salesService,
        EmployeesApiService employeesService,
        ILogger<ReportsController> logger)
    {
        _salesService = salesService;
        _employeesService = employeesService;
        _logger = logger;
    }

    [HttpGet("sales/daily")]
    public async Task<ActionResult<List<SalesReportItem>>> GetDailySalesReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var sales = await _salesService.GetSalesAsync(startDate, endDate) ?? new List<SaleDto>();

            var dailyReport = sales
                .GroupBy(s => s.SaleDate.Date)
                .Select(g => new SalesReportItem(
                    g.Key,
                    g.Sum(s => s.Total),
                    g.Count(),
                    g.Average(s => s.Total)
                ))
                .OrderBy(r => r.Date)
                .ToList();

            return Ok(dailyReport);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating daily sales report");
            return StatusCode(500, "An error occurred while generating the report");
        }
    }

    [HttpGet("sales/by-employee")]
    public async Task<ActionResult<List<SalesByEmployeeItem>>> GetSalesByEmployee(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var sales = await _salesService.GetSalesAsync(startDate, endDate) ?? new List<SaleDto>();
            var employees = await _employeesService.GetAllAsync() ?? new List<EmployeeDto>();

            var employeeMap = employees.ToDictionary(e => e.Id);

            var reportItems = sales
                .GroupBy(s => s.EmployeeId)
                .Select(g =>
                {
                    var employee = employeeMap.GetValueOrDefault(g.Key);
                    var employeeName = employee != null ? $"{employee.FirstName} {employee.LastName}" : "Unknown";
                    var totalSales = g.Sum(s => s.Total);
                    var commission = employee?.CommissionRate != null
                        ? totalSales * (employee.CommissionRate.Value / 100)
                        : 0;

                    return new SalesByEmployeeItem(
                        g.Key,
                        employeeName,
                        totalSales,
                        g.Count(),
                        commission
                    );
                })
                .OrderByDescending(r => r.TotalSales)
                .ToList();

            return Ok(reportItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sales by employee report");
            return StatusCode(500, "An error occurred while generating the report");
        }
    }
}
