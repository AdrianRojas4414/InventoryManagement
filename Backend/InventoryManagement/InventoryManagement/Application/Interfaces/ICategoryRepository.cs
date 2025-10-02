using InventoryManagement.Domain.Entities;
namespace InventoryManagement.Application.Interfaces;

public interface ICategoryRepository
{
    Task AddAsync(Category category);
}