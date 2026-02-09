using MediatR;
using Sales.Domain.Interfaces;
using Sales.Domain.ValueObjects;

namespace Sales.Application.Commands.AddCashMovement;

public class AddCashMovementCommandHandler : IRequestHandler<AddCashMovementCommand, Unit>
{
    private readonly IShiftRepository _shiftRepository;

    public AddCashMovementCommandHandler(IShiftRepository shiftRepository)
    {
        _shiftRepository = shiftRepository;
    }

    public async Task<Unit> Handle(AddCashMovementCommand request, CancellationToken cancellationToken)
    {
        var shift = await _shiftRepository.GetByIdAsync(request.ShiftId);
        if (shift == null)
            throw new InvalidOperationException($"Shift with ID {request.ShiftId} not found");

        var amount = new Money(request.Amount, request.Currency);
        shift.AddCashMovement(request.Type, amount, request.Reason);

        await _shiftRepository.UpdateAsync(shift);

        return Unit.Value;
    }
}
