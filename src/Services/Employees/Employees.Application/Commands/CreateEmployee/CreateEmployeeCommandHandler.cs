using AutoMapper;
using Employees.Application.DTOs;
using Employees.Domain.Entities;
using Employees.Domain.Interfaces;
using Employees.Domain.ValueObjects;
using MediatR;

namespace Employees.Application.Commands;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateEmployeeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var existingEmployee = await _unitOfWork.EmployeeRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingEmployee != null)
            throw new InvalidOperationException($"An employee with email {request.Email} already exists");

        var salary = new Salary(request.SalaryAmount, request.SalaryCurrency);
        Commission? commission = null;

        if (request.CommissionRate.HasValue)
        {
            commission = new Commission(request.CommissionRate.Value, request.CommissionMinimumSales ?? 0);
        }

        var employee = new Employee(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.RoleId,
            salary,
            commission
        );

        await _unitOfWork.EmployeeRepository.AddAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EmployeeDto>(employee);
    }
}
