using MediatR;

namespace Employees.Application.Commands;

public record ClockInCommand(Guid EmployeeId, string? Notes) : IRequest<bool>;
