using Microsoft.AspNetCore.Mvc;
using Reports.API.Models;
using Reports.API.Services;

namespace Reports.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly SalesApiService _salesService;
    private readonly InventoryApiService _inventoryService;
    private readonly CustomersApiService _customersService;
    private readonly EmployeesApiService _employeesService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        SalesApiService salesService,
        InventoryApiService inventoryService,
        CustomersApiService customersService,
        EmployeesApiService employeesService,
        ILogger<DashboardController> logger)
    {
        _salesService = salesService;
        _inventoryService = inventoryService;
        _customersService = customersService;
        _employeesService = employeesService;
        _logger = logger;
    }

    [HttpGet("kpis")]
    public async Task<ActionResult<DashboardKpi>> GetKpis()
    {
        try
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            // Fetch data from all services in parallel
            var salesTask = _salesService.GetSalesAsync(startOfMonth, today.AddDays(1));
            var inventoryTask = _inventoryService.GetAllStockLevelsAsync();
            var customersTask = _customersService.GetAllAsync();
            var employeesTask = _employeesService.GetAllAsync();

            await Task.WhenAll(salesTask, inventoryTask, customersTask, employeesTask);

            var sales = await salesTask ?? new List<SaleDto>();
            var inventory = await inventoryTask ?? new List<InventoryItemDto>();
            var customers = await customersTask ?? new List<CustomerDto>();
            var employees = await employeesTask ?? new List<EmployeeDto>();

            // Calculate KPIs
            var todaySales = sales.Where(s => s.SaleDate.Date == today).Sum(s => s.Total);
            var monthlySales = sales.Sum(s => s.Total);
            var lowStockCount = inventory.Count(i => i.Quantity <= i.ReorderLevel);
            var activeCustomersCount = customers.Count(c => c.IsActive);
            var totalEmployees = employees.Count;
            var todayTransactions = sales.Count(s => s.SaleDate.Date == today);

            var kpi = new DashboardKpi(
                todaySales,
                monthlySales,
                lowStockCount,
                activeCustomersCount,
                totalEmployees,
                todayTransactions
            );

            return Ok(kpi);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating dashboard KPIs");
            return StatusCode(500, "An error occurred while calculating KPIs");
        }
    }
}
