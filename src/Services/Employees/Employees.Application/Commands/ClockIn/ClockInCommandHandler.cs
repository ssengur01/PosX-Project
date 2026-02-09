using Employees.Domain.Interfaces;
using MediatR;

namespace Employees.Application.Commands;

public class ClockInCommandHandler : IRequestHandler<ClockInCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public ClockInCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ClockInCommand request, CancellationToken cancellationToken)
    {
        var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee == null)
            return false;

        employee.ClockIn(request.Notes);

        await _unitOfWork.EmployeeRepository.UpdateAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
