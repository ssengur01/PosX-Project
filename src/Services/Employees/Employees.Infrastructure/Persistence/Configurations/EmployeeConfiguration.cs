using Employees.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Employees.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Phone).IsRequired().HasMaxLength(20);
        builder.Property(e => e.Status).IsRequired();

        builder.OwnsOne(e => e.Salary, salary =>
        {
            salary.Property(s => s.Amount).HasColumnName("SalaryAmount").IsRequired();
            salary.Property(s => s.Currency).HasColumnName("SalaryCurrency").IsRequired().HasMaxLength(10);
        });

        builder.OwnsOne(e => e.Commission, commission =>
        {
            commission.Property(c => c.Rate).HasColumnName("CommissionRate");
            commission.Property(c => c.MinimumSales).HasColumnName("CommissionMinimumSales");
        });

        builder.HasMany(e => e.TimeEntries)
            .WithOne()
            .HasForeignKey(t => t.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.Email).IsUnique();
    }
}
