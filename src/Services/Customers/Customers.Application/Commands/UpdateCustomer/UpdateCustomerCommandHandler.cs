using AutoMapper;
using Customers.Application.DTOs;
using Customers.Domain.Interfaces;
using MediatR;

namespace Customers.Application.Commands;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, CustomerDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCustomerCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CustomerDto?> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(request.Id, cancellationToken);

        if (customer == null)
            return null;

        // Check if phone is being changed and if new phone already exists
        if (customer.Contact.Phone != request.Phone)
        {
            var existingCustomer = await _unitOfWork.CustomerRepository.GetByPhoneAsync(request.Phone, cancellationToken);
            if (existingCustomer != null && existingCustomer.Id != customer.Id)
                throw new InvalidOperationException($"A customer with phone {request.Phone} already exists");
        }

        customer.UpdatePersonalInfo(request.FirstName, request.LastName);
        customer.UpdateContact(request.Phone, request.Email, request.Address);

        await _unitOfWork.CustomerRepository.UpdateAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CustomerDto>(customer);
    }
}
