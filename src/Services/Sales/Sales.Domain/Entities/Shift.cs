using Sales.Domain.ValueObjects;

namespace Sales.Domain.Entities;

public class Shift
{
    public Guid Id { get; private set; }
    public Guid EmployeeId { get; private set; }
    public DateTime OpenedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public Money StartingCash { get; private set; }
    public Money? EndingCash { get; private set; }
    public Money ExpectedCash { get; private set; }
    public Money Difference { get; private set; }
    public bool IsOpen { get; private set; }
    public string? Notes { get; private set; }

    private readonly List<CashMovement> _cashMovements = new();
    public IReadOnlyCollection<CashMovement> CashMovements => _cashMovements.AsReadOnly();

    private Shift()
    {
        StartingCash = Money.Zero();
        ExpectedCash = Money.Zero();
        Difference = Money.Zero();
    }

    public Shift(Guid employeeId, Money startingCash)
    {
        Id = Guid.NewGuid();
        EmployeeId = employeeId;
        OpenedAt = DateTime.UtcNow;
        StartingCash = startingCash ?? throw new ArgumentNullException(nameof(startingCash));
        ExpectedCash = startingCash;
        Difference = Money.Zero();
        IsOpen = true;
    }

    public void AddCashMovement(string type, Money amount, string reason)
    {
        if (!IsOpen)
            throw new InvalidOperationException("Cannot add cash movement to a closed shift");

        var movement = new CashMovement(Id, type, amount, reason);
        _cashMovements.Add(movement);

        // Update expected cash based on movement type
        if (type == "Deposit")
            ExpectedCash += amount;
        else if (type == "Withdrawal")
            ExpectedCash -= amount;
    }

    public void AddSale(Money saleTotal)
    {
        if (!IsOpen)
            throw new InvalidOperationException("Cannot add sale to a closed shift");

        ExpectedCash += saleTotal;
    }

    public void Close(Money endingCash, string? notes = null)
    {
        if (!IsOpen)
            throw new InvalidOperationException("Shift is already closed");

        EndingCash = endingCash ?? throw new ArgumentNullException(nameof(endingCash));
        ClosedAt = DateTime.UtcNow;
        Difference = EndingCash - ExpectedCash;
        Notes = notes;
        IsOpen = false;
    }
}

public class CashMovement
{
    public Guid Id { get; private set; }
    public Guid ShiftId { get; private set; }
    public string Type { get; private set; } // Deposit, Withdrawal
    public Money Amount { get; private set; }
    public string Reason { get; private set; }
    public DateTime Timestamp { get; private set; }

    private CashMovement()
    {
        Type = string.Empty;
        Amount = Money.Zero();
        Reason = string.Empty;
    }

    public CashMovement(Guid shiftId, string type, Money amount, string reason)
    {
        Id = Guid.NewGuid();
        ShiftId = shiftId;
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Reason = reason ?? throw new ArgumentNullException(nameof(reason));
        Timestamp = DateTime.UtcNow;
    }
}
