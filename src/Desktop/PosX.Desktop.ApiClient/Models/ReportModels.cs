namespace PosX.Desktop.ApiClient.Models;

public record DashboardKpiDto(
    decimal TodaySales,
    decimal MonthlySales,
    int LowStockCount,
    int ActiveCustomersCount,
    int TotalEmployees,
    int TodayTransactions
);

public record SalesReportItemDto(
    DateTime Date,
    decimal TotalSales,
    int TransactionCount,
    decimal AverageSale
);

public record SalesByEmployeeItemDto(
    Guid EmployeeId,
    string EmployeeName,
    decimal TotalSales,
    int TransactionCount,
    decimal Commission
);
