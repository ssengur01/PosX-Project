using MediatR;
using Sales.Domain.Interfaces;
using Sales.Domain.ValueObjects;

namespace Sales.Application.Commands.CloseShift;

public class CloseShiftCommandHandler : IRequestHandler<CloseShiftCommand, Unit>
{
    private readonly IShiftRepository _shiftRepository;

    public CloseShiftCommandHandler(IShiftRepository shiftRepository)
    {
        _shiftRepository = shiftRepository;
    }

    public async Task<Unit> Handle(CloseShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await _shiftRepository.GetByIdAsync(request.ShiftId);
        if (shift == null)
            throw new InvalidOperationException($"Shift with ID {request.ShiftId} not found");

        var endingCash = new Money(request.EndingCash, request.Currency);
        shift.Close(endingCash, request.Notes);

        await _shiftRepository.UpdateAsync(shift);

        return Unit.Value;
    }
}
