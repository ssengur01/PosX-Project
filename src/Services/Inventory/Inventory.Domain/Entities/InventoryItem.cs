namespace Inventory.Domain.Entities;

public class InventoryItem
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public int ReorderLevel { get; private set; }
    public DateTime LastUpdated { get; private set; }

    private readonly List<StockMovement> _movements = new();
    public IReadOnlyCollection<StockMovement> Movements => _movements.AsReadOnly();

    private InventoryItem() { }

    public InventoryItem(Guid productId, int initialQuantity, int reorderLevel)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = initialQuantity;
        ReorderLevel = reorderLevel;
        LastUpdated = DateTime.UtcNow;
    }

    public void AddStock(int quantity, string reason, string performedBy)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        Quantity += quantity;
        LastUpdated = DateTime.UtcNow;

        var movement = new StockMovement(Id, Enums.MovementType.StockIn, quantity, reason, performedBy);
        _movements.Add(movement);
    }

    public void RemoveStock(int quantity, string reason, string performedBy)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (Quantity < quantity)
            throw new InvalidOperationException("Insufficient stock");

        Quantity -= quantity;
        LastUpdated = DateTime.UtcNow;

        var movement = new StockMovement(Id, Enums.MovementType.StockOut, -quantity, reason, performedBy);
        _movements.Add(movement);
    }

    public void AdjustStock(int newQuantity, string reason, string performedBy)
    {
        if (newQuantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(newQuantity));

        var difference = newQuantity - Quantity;
        Quantity = newQuantity;
        LastUpdated = DateTime.UtcNow;

        var movement = new StockMovement(Id, Enums.MovementType.Adjustment, difference, reason, performedBy);
        _movements.Add(movement);
    }

    public void ProcessSale(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (Quantity < quantity)
            throw new InvalidOperationException("Insufficient stock for sale");

        Quantity -= quantity;
        LastUpdated = DateTime.UtcNow;

        var movement = new StockMovement(Id, Enums.MovementType.Sale, -quantity, "Sale transaction", "System");
        _movements.Add(movement);
    }

    public bool IsLowStock() => Quantity <= ReorderLevel;

    public void UpdateReorderLevel(int newLevel)
    {
        if (newLevel < 0)
            throw new ArgumentException("Reorder level cannot be negative", nameof(newLevel));

        ReorderLevel = newLevel;
        LastUpdated = DateTime.UtcNow;
    }
}
