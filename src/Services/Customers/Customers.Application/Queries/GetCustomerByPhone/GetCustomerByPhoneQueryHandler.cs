using AutoMapper;
using Customers.Application.DTOs;
using Customers.Domain.Interfaces;
using MediatR;

namespace Customers.Application.Queries.GetCustomerByPhone;

public class GetCustomerByPhoneQueryHandler : IRequestHandler<GetCustomerByPhoneQuery, CustomerDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCustomerByPhoneQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByPhoneQuery request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByPhoneAsync(request.Phone, cancellationToken);
        return customer == null ? null : _mapper.Map<CustomerDto>(customer);
    }
}
