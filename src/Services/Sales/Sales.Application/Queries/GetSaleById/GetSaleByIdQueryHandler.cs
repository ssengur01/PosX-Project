using AutoMapper;
using MediatR;
using Sales.Application.DTOs;
using Sales.Domain.Interfaces;

namespace Sales.Application.Queries.GetSaleById;

public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, SaleDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSaleByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SaleDto?> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        var sale = await _unitOfWork.SaleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        return sale == null ? null : _mapper.Map<SaleDto>(sale);
    }
}
