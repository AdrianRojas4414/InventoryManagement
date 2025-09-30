using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(short id);
    Task<List<Supplier>> GetAllAsync();
    Task AddAsync(Supplier supplier);
    Task UpdateAsync(Supplier supplier);
}