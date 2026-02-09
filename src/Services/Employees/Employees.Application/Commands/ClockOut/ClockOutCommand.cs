using MediatR;

namespace Employees.Application.Commands;

public record ClockOutCommand(Guid EmployeeId) : IRequest<bool>;
