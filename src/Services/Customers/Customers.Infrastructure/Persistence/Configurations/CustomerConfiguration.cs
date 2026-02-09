using Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customers.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);

        builder.OwnsOne(c => c.Contact, contact =>
        {
            contact.Property(ci => ci.Phone).HasColumnName("Phone").IsRequired().HasMaxLength(20);
            contact.Property(ci => ci.Email).HasColumnName("Email").HasMaxLength(200);
            contact.Property(ci => ci.Address).HasColumnName("Address").HasMaxLength(500);
        });

        builder.HasIndex(c => c.Contact.Phone);
    }
}
