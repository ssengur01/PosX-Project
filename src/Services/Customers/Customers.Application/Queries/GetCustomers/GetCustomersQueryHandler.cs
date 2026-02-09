using AutoMapper;
using Customers.Application.DTOs;
using Customers.Domain.Interfaces;
using MediatR;

namespace Customers.Application.Queries.GetCustomers;

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, List<CustomerDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCustomersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = string.IsNullOrWhiteSpace(request.SearchTerm)
            ? await _unitOfWork.CustomerRepository.GetAllAsync(cancellationToken)
            : await _unitOfWork.CustomerRepository.SearchAsync(request.SearchTerm, cancellationToken);

        return _mapper.Map<List<CustomerDto>>(customers);
    }
}
