using Microsoft.EntityFrameworkCore;
using Sales.Domain.Entities;
using Sales.Domain.Interfaces;

namespace Sales.Infrastructure.Persistence.Repositories;

public class ShiftRepository : IShiftRepository
{
    private readonly SalesDbContext _context;

    public ShiftRepository(SalesDbContext context)
    {
        _context = context;
    }

    public async Task<Shift?> GetByIdAsync(Guid id)
    {
        return await _context.Shifts
            .Include(s => s.CashMovements)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Shift?> GetOpenShiftByEmployeeAsync(Guid employeeId)
    {
        return await _context.Shifts
            .Include(s => s.CashMovements)
            .FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.IsOpen);
    }

    public async Task<List<Shift>> GetShiftsByEmployeeAsync(Guid employeeId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Shifts
            .Include(s => s.CashMovements)
            .Where(s => s.EmployeeId == employeeId);

        if (startDate.HasValue)
            query = query.Where(s => s.OpenedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(s => s.OpenedAt <= endDate.Value);

        return await query
            .OrderByDescending(s => s.OpenedAt)
            .ToListAsync();
    }

    public async Task<Shift> AddAsync(Shift shift)
    {
        await _context.Shifts.AddAsync(shift);
        await _context.SaveChangesAsync();
        return shift;
    }

    public async Task UpdateAsync(Shift shift)
    {
        _context.Shifts.Update(shift);
        await _context.SaveChangesAsync();
    }
}
