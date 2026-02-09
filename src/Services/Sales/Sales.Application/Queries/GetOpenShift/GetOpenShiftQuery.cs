using MediatR;
using Sales.Application.DTOs;

namespace Sales.Application.Queries.GetOpenShift;

public record GetOpenShiftQuery(Guid EmployeeId) : IRequest<ShiftDto?>;
