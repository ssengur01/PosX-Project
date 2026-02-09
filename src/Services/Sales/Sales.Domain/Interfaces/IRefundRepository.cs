using Sales.Domain.Entities;

namespace Sales.Domain.Interfaces;

public interface IRefundRepository
{
    Task<Refund?> GetByIdAsync(Guid id);
    Task<List<Refund>> GetBySaleIdAsync(Guid saleId);
    Task<List<Refund>> GetAllAsync();
    Task<Refund> AddAsync(Refund refund);
}
