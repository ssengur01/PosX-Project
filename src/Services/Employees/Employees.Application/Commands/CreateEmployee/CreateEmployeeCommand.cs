using Employees.Application.DTOs;
using MediatR;

namespace Employees.Application.Commands;

public record CreateEmployeeCommand(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    Guid RoleId,
    decimal SalaryAmount,
    string SalaryCurrency,
    decimal? CommissionRate,
    decimal? CommissionMinimumSales
) : IRequest<EmployeeDto>;
