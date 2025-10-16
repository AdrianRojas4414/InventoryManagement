using InventoryManagement.Domain.Entities;
namespace InventoryManagement.Application.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(short id);
    Task<List<Category>> GetAllAsync();
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(short id);
}