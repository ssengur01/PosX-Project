namespace Employees.Domain.Interfaces;

public interface IUnitOfWork
{
    IEmployeeRepository EmployeeRepository { get; }
    IRoleRepository RoleRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
