namespace Reports.API.Models;

public record DashboardKpi(
    decimal TodaySales,
    decimal MonthlySales,
    int LowStockCount,
    int ActiveCustomersCount,
    int TotalEmployees,
    int TodayTransactions
);

public record SalesReportItem(
    DateTime Date,
    decimal TotalSales,
    int TransactionCount,
    decimal AverageSale
);

public record SalesByEmployeeItem(
    Guid EmployeeId,
    string EmployeeName,
    decimal TotalSales,
    int TransactionCount,
    decimal Commission
);

public record SalesByProductItem(
    Guid ProductId,
    string ProductName,
    int QuantitySold,
    decimal TotalRevenue
);

public record InventoryStatusItem(
    Guid ProductId,
    string ProductName,
    int CurrentStock,
    int ReorderLevel,
    bool IsLowStock
);
