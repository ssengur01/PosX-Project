using Employees.Application.DTOs;
using MediatR;

namespace Employees.Application.Queries;

public record GetAllEmployeesQuery : IRequest<List<EmployeeDto>>;
