using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly InventoryDbContext _context;

    public InventoryRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryItem?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems
            .Include(i => i.Movements)
            .FirstOrDefaultAsync(i => i.ProductId == productId, cancellationToken);
    }

    public async Task<List<InventoryItem>> GetLowStockItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems
            .Where(i => i.Quantity <= i.ReorderLevel)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<InventoryItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems.ToListAsync(cancellationToken);
    }

    public async Task<List<StockMovement>> GetMovementsAsync(Guid inventoryItemId, CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements
            .Where(m => m.InventoryItemId == inventoryItemId)
            .OrderByDescending(m => m.MovementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        await _context.InventoryItems.AddAsync(item, cancellationToken);
    }

    public Task UpdateAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        _context.InventoryItems.Update(item);
        return Task.CompletedTask;
    }
}
