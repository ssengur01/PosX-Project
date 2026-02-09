namespace Inventory.Domain.Interfaces;

public interface IUnitOfWork
{
    IInventoryRepository InventoryRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
