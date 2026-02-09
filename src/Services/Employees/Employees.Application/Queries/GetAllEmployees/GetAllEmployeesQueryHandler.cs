using AutoMapper;
using Employees.Application.DTOs;
using Employees.Domain.Interfaces;
using MediatR;

namespace Employees.Application.Queries;

public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, List<EmployeeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllEmployeesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<EmployeeDto>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _unitOfWork.EmployeeRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<EmployeeDto>>(employees);
    }
}
