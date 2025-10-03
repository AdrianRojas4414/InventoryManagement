using InventoryManagement.Domain.Entities;
namespace InventoryManagement.Application.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task AddAsync(Product product);
}