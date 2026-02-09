using Employees.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Employees.Infrastructure.Persistence.Configurations;

public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("TimeEntries");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.ClockInTime).IsRequired();
        builder.Property(t => t.ClockOutTime);
        builder.Property(t => t.Notes).HasMaxLength(500);

        builder.HasIndex(t => t.EmployeeId);
    }
}
