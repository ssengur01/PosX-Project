using Microsoft.EntityFrameworkCore;
using Products.Domain.Entities;
using Products.Domain.Interfaces;
using Products.Infrastructure.Persistence;

namespace Products.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ProductsDbContext _context;

    public CategoryRepository(ProductsDbContext context) => _context = context;

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Categories.FindAsync(new object[] { id }, cancellationToken);

    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
        await _context.Categories.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Categories.OrderBy(c => c.Name).ToListAsync(cancellationToken);

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default) =>
        await _context.Categories.AddAsync(category, cancellationToken);

    public void Update(Category category) => _context.Categories.Update(category);
    public void Delete(Category category) => _context.Categories.Remove(category);
}
