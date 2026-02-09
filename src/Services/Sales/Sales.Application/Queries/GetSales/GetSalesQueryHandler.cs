using AutoMapper;
using MediatR;
using Sales.Application.DTOs;
using Sales.Domain.Interfaces;

namespace Sales.Application.Queries.GetSales;

public class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, List<SaleDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSalesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<SaleDto>> Handle(GetSalesQuery request, CancellationToken cancellationToken)
    {
        var sales = request.EmployeeId.HasValue
            ? await _unitOfWork.SaleRepository.GetByEmployeeIdAsync(request.EmployeeId.Value, request.StartDate, request.EndDate, cancellationToken)
            : await _unitOfWork.SaleRepository.GetByDateRangeAsync(request.StartDate ?? DateTime.UtcNow.Date, request.EndDate ?? DateTime.UtcNow.Date.AddDays(1), cancellationToken);

        return _mapper.Map<List<SaleDto>>(sales);
    }
}
