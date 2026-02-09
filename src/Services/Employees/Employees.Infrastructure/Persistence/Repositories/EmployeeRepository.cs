using Employees.Domain.Entities;
using Employees.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Employees.Infrastructure.Persistence.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly EmployeesDbContext _context;

    public EmployeeRepository(EmployeesDbContext context)
    {
        _context = context;
    }

    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.TimeEntries)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.TimeEntries)
            .FirstOrDefaultAsync(e => e.Email == email, cancellationToken);
    }

    public async Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.TimeEntries)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Employee>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.TimeEntries)
            .Where(e => e.RoleId == roleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Employee>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.TimeEntries)
            .Where(e => e.FirstName.Contains(searchTerm) || e.LastName.Contains(searchTerm) || e.Email.Contains(searchTerm))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await _context.Employees.AddAsync(employee, cancellationToken);
    }

    public Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _context.Employees.Update(employee);
        return Task.CompletedTask;
    }
}
