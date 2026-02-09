using Inventory.Domain.Interfaces;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly InventoryDbContext _context;
    private IInventoryRepository? _inventoryRepository;

    public UnitOfWork(InventoryDbContext context)
    {
        _context = context;
    }

    public IInventoryRepository InventoryRepository => _inventoryRepository ??= new InventoryRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
