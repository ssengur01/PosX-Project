using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Products.Domain.Entities;

namespace Products.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(1000);
        builder.Property(p => p.SKU).IsRequired().HasMaxLength(50);

        builder.OwnsOne(p => p.Barcode, barcode =>
        {
            barcode.Property(b => b.Value).HasColumnName("Barcode").HasMaxLength(13);
        });

        builder.OwnsOne(p => p.Price, price =>
        {
            price.Property(m => m.Amount).HasColumnName("Price").HasColumnType("decimal(18,2)");
            price.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.OwnsOne(p => p.Cost, cost =>
        {
            cost.Property(m => m.Amount).HasColumnName("Cost").HasColumnType("decimal(18,2)");
            cost.Property(m => m.Currency).HasColumnName("CostCurrency").HasMaxLength(3);
        });

        builder.Property(p => p.StockQuantity).IsRequired();
        builder.Property(p => p.MinimumStockLevel).IsRequired();
        builder.Property(p => p.IsActive).IsRequired();

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.SKU).IsUnique();
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.IsActive);
    }
}
