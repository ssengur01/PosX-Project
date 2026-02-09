using AutoMapper;
using Customers.Application.DTOs;
using Customers.Domain.Interfaces;
using MediatR;

namespace Customers.Application.Commands;

public class RedeemLoyaltyPointsCommandHandler : IRequestHandler<RedeemLoyaltyPointsCommand, CustomerDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RedeemLoyaltyPointsCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CustomerDto?> Handle(RedeemLoyaltyPointsCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(request.CustomerId, cancellationToken);

        if (customer == null)
            return null;

        customer.RedeemLoyaltyPoints(request.Points);

        await _unitOfWork.CustomerRepository.UpdateAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CustomerDto>(customer);
    }
}
