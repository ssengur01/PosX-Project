using Customers.Domain.ValueObjects;

namespace Customers.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public ContactInfo Contact { get; private set; }
    public int LoyaltyPoints { get; private set; }
    public DateTime JoinedDate { get; private set; }
    public DateTime? LastPurchaseDate { get; private set; }
    public bool IsActive { get; private set; }

    private Customer()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Contact = new ContactInfo(string.Empty, null, null);
    }

    public Customer(string firstName, string lastName, string phone, string? email = null, string? address = null)
    {
        Id = Guid.NewGuid();
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Contact = new ContactInfo(phone, email, address);
        LoyaltyPoints = 0;
        JoinedDate = DateTime.UtcNow;
        IsActive = true;
    }

    public void UpdatePersonalInfo(string firstName, string lastName)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
    }

    public void UpdateContact(string phone, string? email, string? address)
    {
        Contact = new ContactInfo(phone, email, address);
    }

    public void AddLoyaltyPoints(int points)
    {
        if (points < 0)
            throw new ArgumentException("Points cannot be negative", nameof(points));

        LoyaltyPoints += points;
    }

    public void RedeemLoyaltyPoints(int points)
    {
        if (points < 0)
            throw new ArgumentException("Points cannot be negative", nameof(points));

        if (LoyaltyPoints < points)
            throw new InvalidOperationException("Insufficient loyalty points");

        LoyaltyPoints -= points;
    }

    public void RecordPurchase()
    {
        LastPurchaseDate = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public string GetFullName() => $"{FirstName} {LastName}";
}
