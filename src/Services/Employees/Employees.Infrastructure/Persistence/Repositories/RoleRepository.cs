using Employees.Domain.Entities;
using Employees.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Employees.Infrastructure.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly EmployeesDbContext _context;

    public RoleRepository(EmployeesDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Roles.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        await _context.Roles.AddAsync(role, cancellationToken);
    }

    public Task UpdateAsync(Role role, CancellationToken cancellationToken = default)
    {
        _context.Roles.Update(role);
        return Task.CompletedTask;
    }
}
