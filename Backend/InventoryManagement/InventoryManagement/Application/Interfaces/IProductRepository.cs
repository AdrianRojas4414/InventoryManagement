using InventoryManagement.Domain.Entities;
namespace InventoryManagement.Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(short id);
    Task<List<Product>> GetAllAsync();
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(short id);
}