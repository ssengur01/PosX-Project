using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;

public interface IInventoryRepository
{
    Task<InventoryItem?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<List<InventoryItem>> GetLowStockItemsAsync(CancellationToken cancellationToken = default);
    Task<List<InventoryItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<StockMovement>> GetMovementsAsync(Guid inventoryItemId, CancellationToken cancellationToken = default);
    Task AddAsync(InventoryItem item, CancellationToken cancellationToken = default);
    Task UpdateAsync(InventoryItem item, CancellationToken cancellationToken = default);
}
