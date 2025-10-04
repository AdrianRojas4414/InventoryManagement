using InventoryManagement.Domain.Entities;
namespace InventoryManagement.Application.Interfaces;

public interface IPurchaseDetailRepository
{
    Task<List<PurchaseDetail>> GetAllAsync();

    Task<List<PurchaseDetail>> GetByPurchaseIdAsync(int purchaseId);

    Task AddAsync(PurchaseDetail purchaseDetail);

    Task UpdateAsync(PurchaseDetail purchaseDetail);
}