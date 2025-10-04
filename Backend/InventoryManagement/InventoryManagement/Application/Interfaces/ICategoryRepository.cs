using InventoryManagement.Domain.Entities;
namespace InventoryManagement.Application.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task AddAsync(Category category);
}