using Microsoft.EntityFrameworkCore;
using Products.Domain.Entities;
using Products.Domain.Interfaces;
using Products.Infrastructure.Persistence;

namespace Products.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductsDbContext _context;

    public ProductRepository(ProductsDbContext context) => _context = context;

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default) =>
        await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.SKU == sku, cancellationToken);

    public async Task<IEnumerable<Product>> GetAllAsync(int skip, int take, CancellationToken cancellationToken = default) =>
        await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default) =>
        await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .Where(p => p.Name.Contains(searchTerm) ||
                       p.SKU.Contains(searchTerm) ||
                       (p.Barcode != null && p.Barcode.Value.Contains(searchTerm)))
            .Take(50) // Limit results for performance
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default) =>
        await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Product>> GetLowStockAsync(CancellationToken cancellationToken = default) =>
        await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .Where(p => p.StockQuantity <= p.MinimumStockLevel)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default) =>
        await _context.Products.AddAsync(product, cancellationToken);

    public void Update(Product product) => _context.Products.Update(product);
    public void Delete(Product product) => _context.Products.Remove(product);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        await _context.Products
            .AsNoTracking()
            .CountAsync(cancellationToken);
}
