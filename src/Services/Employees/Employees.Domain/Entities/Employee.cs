using Employees.Domain.Enums;
using Employees.Domain.ValueObjects;

namespace Employees.Domain.Entities;

public class Employee
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public Guid RoleId { get; private set; }
    public Salary Salary { get; private set; }
    public Commission? Commission { get; private set; }
    public DateTime HireDate { get; private set; }
    public EmployeeStatus Status { get; private set; }
    private readonly List<TimeEntry> _timeEntries = new();
    public IReadOnlyCollection<TimeEntry> TimeEntries => _timeEntries.AsReadOnly();

    private Employee()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        Phone = string.Empty;
        Salary = new Salary(0);
    }

    public Employee(string firstName, string lastName, string email, string phone, Guid roleId, Salary salary, Commission? commission = null)
    {
        Id = Guid.NewGuid();
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        RoleId = roleId;
        Salary = salary ?? throw new ArgumentNullException(nameof(salary));
        Commission = commission;
        HireDate = DateTime.UtcNow;
        Status = EmployeeStatus.Active;
    }

    public void UpdatePersonalInfo(string firstName, string lastName, string email, string phone)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
    }

    public void UpdateSalary(Salary salary)
    {
        Salary = salary ?? throw new ArgumentNullException(nameof(salary));
    }

    public void UpdateCommission(Commission? commission)
    {
        Commission = commission;
    }

    public void AssignRole(Guid roleId)
    {
        RoleId = roleId;
    }

    public void ClockIn(string? notes = null)
    {
        if (Status != EmployeeStatus.Active)
            throw new InvalidOperationException("Only active employees can clock in");

        var lastEntry = _timeEntries.OrderByDescending(t => t.ClockInTime).FirstOrDefault();
        if (lastEntry != null && !lastEntry.ClockOutTime.HasValue)
            throw new InvalidOperationException("Employee is already clocked in");

        var entry = new TimeEntry(Id, DateTime.UtcNow, notes);
        _timeEntries.Add(entry);
    }

    public void ClockOut()
    {
        var currentEntry = _timeEntries.OrderByDescending(t => t.ClockInTime).FirstOrDefault();
        if (currentEntry == null || currentEntry.ClockOutTime.HasValue)
            throw new InvalidOperationException("Employee is not clocked in");

        currentEntry.ClockOut(DateTime.UtcNow);
    }

    public void Activate()
    {
        Status = EmployeeStatus.Active;
    }

    public void Deactivate()
    {
        Status = EmployeeStatus.Inactive;
    }

    public void SetOnLeave()
    {
        Status = EmployeeStatus.OnLeave;
    }

    public void Terminate()
    {
        Status = EmployeeStatus.Terminated;
    }

    public string GetFullName() => $"{FirstName} {LastName}";

    public decimal CalculateMonthlyCommission(decimal monthlySales)
    {
        return Commission?.Calculate(monthlySales) ?? 0;
    }
}
