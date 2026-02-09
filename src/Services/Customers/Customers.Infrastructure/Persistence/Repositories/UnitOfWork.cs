using Customers.Domain.Interfaces;

namespace Customers.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly CustomersDbContext _context;
    private ICustomerRepository? _customerRepository;

    public UnitOfWork(CustomersDbContext context)
    {
        _context = context;
    }

    public ICustomerRepository CustomerRepository => _customerRepository ??= new CustomerRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
