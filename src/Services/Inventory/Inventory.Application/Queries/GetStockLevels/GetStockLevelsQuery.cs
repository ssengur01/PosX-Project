using Inventory.Application.DTOs;
using MediatR;

namespace Inventory.Application.Queries.GetStockLevels;

public record GetStockLevelsQuery(Guid? ProductId = null) : IRequest<List<InventoryDto>>;
