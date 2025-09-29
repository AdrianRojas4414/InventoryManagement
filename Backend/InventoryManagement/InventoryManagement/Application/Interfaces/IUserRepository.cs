using InventoryManagement.Domain.Entities;
namespace InventoryManagement.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(ushort id);
        Task<User?> GetByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> AddAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> DeactivateAsync(ushort id);
        Task<bool> ActivateAsync(ushort id);
    }
}