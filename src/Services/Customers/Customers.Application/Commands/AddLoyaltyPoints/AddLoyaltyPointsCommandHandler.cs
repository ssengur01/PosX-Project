using Customers.Domain.Interfaces;
using MediatR;

namespace Customers.Application.Commands.AddLoyaltyPoints;

public class AddLoyaltyPointsCommandHandler : IRequestHandler<AddLoyaltyPointsCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddLoyaltyPointsCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AddLoyaltyPointsCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null) return false;

        customer.AddLoyaltyPoints(request.Points);
        await _unitOfWork.CustomerRepository.UpdateAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
