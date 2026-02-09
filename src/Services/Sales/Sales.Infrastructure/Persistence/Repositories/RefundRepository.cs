using Microsoft.EntityFrameworkCore;
using Sales.Domain.Interfaces;
using Sales.Domain.Entities;

namespace Sales.Infrastructure.Persistence.Repositories;

public class RefundRepository : IRefundRepository
{
    private readonly SalesDbContext _context;

    public RefundRepository(SalesDbContext context)
    {
        _context = context;
    }

    public async Task<Refund?> GetByIdAsync(Guid id)
    {
        return await _context.Refunds.FindAsync(id);
    }

    public async Task<List<Refund>> GetBySaleIdAsync(Guid saleId)
    {
        return await _context.Refunds
            .Where(r => r.SaleId == saleId)
            .OrderByDescending(r => r.RefundDate)
            .ToListAsync();
    }

    public async Task<List<Refund>> GetAllAsync()
    {
        return await _context.Refunds
            .OrderByDescending(r => r.RefundDate)
            .ToListAsync();
    }

    public async Task<Refund> AddAsync(Refund refund)
    {
        await _context.Refunds.AddAsync(refund);
        await _context.SaveChangesAsync();
        return refund;
    }
}
