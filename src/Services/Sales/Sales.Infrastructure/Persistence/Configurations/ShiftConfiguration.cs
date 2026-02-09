using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Domain.Entities;

namespace Sales.Infrastructure.Persistence.Configurations;

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.ToTable("Shifts");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.EmployeeId).IsRequired();
        builder.Property(s => s.OpenedAt).IsRequired();
        builder.Property(s => s.IsOpen).IsRequired();
        builder.Property(s => s.Notes).HasMaxLength(500);

        // Configure Money value objects
        builder.OwnsOne(s => s.StartingCash, money =>
        {
            money.Property(m => m.Amount).HasColumnName("StartingCash").HasPrecision(18, 2).IsRequired();
            money.Property(m => m.Currency).HasColumnName("StartingCashCurrency").HasMaxLength(3).IsRequired();
        });

        builder.OwnsOne(s => s.EndingCash, money =>
        {
            money.Property(m => m.Amount).HasColumnName("EndingCash").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("EndingCashCurrency").HasMaxLength(3);
        });

        builder.OwnsOne(s => s.ExpectedCash, money =>
        {
            money.Property(m => m.Amount).HasColumnName("ExpectedCash").HasPrecision(18, 2).IsRequired();
            money.Property(m => m.Currency).HasColumnName("ExpectedCashCurrency").HasMaxLength(3).IsRequired();
        });

        builder.OwnsOne(s => s.Difference, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Difference").HasPrecision(18, 2).IsRequired();
            money.Property(m => m.Currency).HasColumnName("DifferenceCurrency").HasMaxLength(3).IsRequired();
        });

        // Configure CashMovements collection
        builder.OwnsMany(s => s.CashMovements, cm =>
        {
            cm.ToTable("CashMovements");
            cm.WithOwner().HasForeignKey("ShiftId");
            cm.HasKey("Id");

            cm.Property<Guid>("Id");
            cm.Property(c => c.ShiftId).IsRequired();
            cm.Property(c => c.Type).IsRequired().HasMaxLength(50);
            cm.Property(c => c.Reason).IsRequired().HasMaxLength(500);
            cm.Property(c => c.Timestamp).IsRequired();

            // Configure Money value object for Amount
            cm.OwnsOne(c => c.Amount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("Amount").HasPrecision(18, 2).IsRequired();
                money.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3).IsRequired();
            });
        });
    }
}
