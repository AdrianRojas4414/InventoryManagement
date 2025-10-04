using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Entities;
namespace InventoryManagement.Application.Interfaces;

public interface IPurchaseRepository
{
    Task AddAsync(CreatePurchaseDto dto, short userId);
    Task<List<Purchase>> GetAllAsync();
    Task<List<Purchase>> GetByUserIdAsync(short userId);
    Task<Purchase?> GetByIdAsync(int purchaseId);
    Task UpdateAsync(Purchase purchase);
}

