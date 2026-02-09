using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Domain.Entities;

namespace Sales.Infrastructure.Persistence.Configurations;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductName).IsRequired().HasMaxLength(200);

        builder.OwnsOne(i => i.UnitPrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("UnitPrice").HasPrecision(18, 2);
        });

        builder.OwnsOne(i => i.Subtotal, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Subtotal").HasPrecision(18, 2);
        });

        builder.OwnsOne(i => i.TaxRate, tax =>
        {
            tax.Property(t => t.Rate).HasColumnName("TaxRate").HasPrecision(5, 2);
            tax.Property(t => t.Name).HasColumnName("TaxName").HasMaxLength(50);
        });

        builder.OwnsOne(i => i.TaxAmount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("TaxAmount").HasPrecision(18, 2);
        });

        builder.OwnsOne(i => i.Discount, discount =>
        {
            discount.Property(d => d.Amount).HasColumnName("DiscountAmount").HasPrecision(18, 2);
            discount.Property(d => d.Type).HasColumnName("DiscountType").HasConversion<string>();
        });

        builder.OwnsOne(i => i.DiscountAmount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("DiscountValue").HasPrecision(18, 2);
        });

        builder.OwnsOne(i => i.Total, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Total").HasPrecision(18, 2);
        });
    }
}
