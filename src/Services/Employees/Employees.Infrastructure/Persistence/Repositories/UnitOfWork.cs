using Employees.Domain.Interfaces;

namespace Employees.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly EmployeesDbContext _context;
    private IEmployeeRepository? _employeeRepository;
    private IRoleRepository? _roleRepository;

    public UnitOfWork(EmployeesDbContext context)
    {
        _context = context;
    }

    public IEmployeeRepository EmployeeRepository => _employeeRepository ??= new EmployeeRepository(_context);
    public IRoleRepository RoleRepository => _roleRepository ??= new RoleRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
