namespace Employees.Domain.Entities;

public class TimeEntry
{
    public Guid Id { get; private set; }
    public Guid EmployeeId { get; private set; }
    public DateTime ClockInTime { get; private set; }
    public DateTime? ClockOutTime { get; private set; }
    public string? Notes { get; private set; }

    private TimeEntry() { }

    public TimeEntry(Guid employeeId, DateTime clockInTime, string? notes = null)
    {
        Id = Guid.NewGuid();
        EmployeeId = employeeId;
        ClockInTime = clockInTime;
        Notes = notes;
    }

    public void ClockOut(DateTime clockOutTime)
    {
        if (ClockOutTime.HasValue)
            throw new InvalidOperationException("Already clocked out");

        if (clockOutTime < ClockInTime)
            throw new ArgumentException("Clock out time cannot be before clock in time", nameof(clockOutTime));

        ClockOutTime = clockOutTime;
    }

    public TimeSpan GetDuration()
    {
        if (!ClockOutTime.HasValue)
            return TimeSpan.Zero;

        return ClockOutTime.Value - ClockInTime;
    }

    public decimal GetHoursWorked()
    {
        return (decimal)GetDuration().TotalHours;
    }
}
