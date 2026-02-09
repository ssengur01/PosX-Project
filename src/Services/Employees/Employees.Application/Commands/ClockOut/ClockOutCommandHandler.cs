using Employees.Domain.Interfaces;
using MediatR;

namespace Employees.Application.Commands;

public class ClockOutCommandHandler : IRequestHandler<ClockOutCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public ClockOutCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ClockOutCommand request, CancellationToken cancellationToken)
    {
        var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee == null)
            return false;

        employee.ClockOut();

        await _unitOfWork.EmployeeRepository.UpdateAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
