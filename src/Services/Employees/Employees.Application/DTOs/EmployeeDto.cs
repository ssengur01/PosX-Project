using Employees.Domain.Enums;

namespace Employees.Application.DTOs;

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
    EmployeeStatus Status
);

public record RoleDto(
    Guid Id,
    string Name,
    string? Description,
    List<PermissionType> Permissions
);

public record TimeEntryDto(
    Guid Id,
    Guid EmployeeId,
    DateTime ClockInTime,
    DateTime? ClockOutTime,
    decimal HoursWorked,
    string? Notes
);
