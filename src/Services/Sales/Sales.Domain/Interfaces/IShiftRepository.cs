using Sales.Domain.Entities;

namespace Sales.Domain.Interfaces;

public interface IShiftRepository
{
    Task<Shift?> GetByIdAsync(Guid id);
    Task<Shift?> GetOpenShiftByEmployeeAsync(Guid employeeId);
    Task<List<Shift>> GetShiftsByEmployeeAsync(Guid employeeId, DateTime? startDate = null, DateTime? endDate = null);
    Task<Shift> AddAsync(Shift shift);
    Task UpdateAsync(Shift shift);
}
