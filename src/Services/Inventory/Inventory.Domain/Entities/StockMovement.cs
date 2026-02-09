using Inventory.Domain.Enums;

namespace Inventory.Domain.Entities;

public class StockMovement
{
    public Guid Id { get; private set; }
    public Guid InventoryItemId { get; private set; }
    public MovementType Type { get; private set; }
    public int Quantity { get; private set; }
    public string Reason { get; private set; }
    public string PerformedBy { get; private set; }
    public DateTime MovementDate { get; private set; }

    private StockMovement()
    {
        Reason = string.Empty;
        PerformedBy = string.Empty;
    }

    public StockMovement(Guid inventoryItemId, MovementType type, int quantity, string reason, string performedBy)
    {
        Id = Guid.NewGuid();
        InventoryItemId = inventoryItemId;
        Type = type;
        Quantity = quantity;
        Reason = reason ?? throw new ArgumentNullException(nameof(reason));
        PerformedBy = performedBy ?? throw new ArgumentNullException(nameof(performedBy));
        MovementDate = DateTime.UtcNow;
    }
}
