using Employees.Application.DTOs;
using MediatR;

namespace Employees.Application.Queries;

public record GetEmployeeByIdQuery(Guid Id) : IRequest<EmployeeDto?>;
