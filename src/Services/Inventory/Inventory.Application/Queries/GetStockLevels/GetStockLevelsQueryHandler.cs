using AutoMapper;
using Inventory.Application.DTOs;
using Inventory.Domain.Interfaces;
using MediatR;

namespace Inventory.Application.Queries.GetStockLevels;

public class GetStockLevelsQueryHandler : IRequestHandler<GetStockLevelsQuery, List<InventoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetStockLevelsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<InventoryDto>> Handle(GetStockLevelsQuery request, CancellationToken cancellationToken)
    {
        if (request.ProductId.HasValue)
        {
            var item = await _unitOfWork.InventoryRepository.GetByProductIdAsync(request.ProductId.Value, cancellationToken);
            return item == null ? new List<InventoryDto>() : new List<InventoryDto> { _mapper.Map<InventoryDto>(item) };
        }

        var items = await _unitOfWork.InventoryRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<InventoryDto>>(items);
    }
}
