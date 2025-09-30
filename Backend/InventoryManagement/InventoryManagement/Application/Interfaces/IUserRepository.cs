using InventoryManagement.Domain.Entities;
namespace InventoryManagement.Application.Interfaces;
public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetByIdAsync(short id);
}