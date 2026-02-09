namespace Customers.Domain.Interfaces;

public interface IUnitOfWork
{
    ICustomerRepository CustomerRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
