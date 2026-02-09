using MediatR;
using Sales.Domain.Entities;
using Sales.Domain.Interfaces;
using Sales.Domain.ValueObjects;

namespace Sales.Application.Commands.OpenShift;

public class OpenShiftCommandHandler : IRequestHandler<OpenShiftCommand, Guid>
{
    private readonly IShiftRepository _shiftRepository;

    public OpenShiftCommandHandler(IShiftRepository shiftRepository)
    {
        _shiftRepository = shiftRepository;
    }

    public async Task<Guid> Handle(OpenShiftCommand request, CancellationToken cancellationToken)
    {
        // Check if employee already has an open shift
        var existingShift = await _shiftRepository.GetOpenShiftByEmployeeAsync(request.EmployeeId);
        if (existingShift != null)
            throw new InvalidOperationException("Employee already has an open shift");

        var startingCash = new Money(request.StartingCash, request.Currency);
        var shift = new Shift(request.EmployeeId, startingCash);

        await _shiftRepository.AddAsync(shift);

        return shift.Id;
    }
}
