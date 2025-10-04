using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

public class PurchaseDetailRepository : IPurchaseDetailRepository
{
    private readonly InventoryDbContext _context;

    public PurchaseDetailRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<List<PurchaseDetail>> GetAllAsync()
    {
        return await _context.PurchaseDetails
            .Include(pd => pd.Product)
            .Include(pd => pd.Purchase)
            .ToListAsync();
    }

    public async Task<List<PurchaseDetail>> GetByPurchaseIdAsync(int purchaseId)
    {
        return await _context.PurchaseDetails
            .Where(pd => pd.PurchaseId == purchaseId)
            .Include(pd => pd.Product)
            .Include(pd => pd.Purchase)
            .ToListAsync();
    }

    public async Task AddAsync(PurchaseDetail purchaseDetail)
    {
        await _context.PurchaseDetails.AddAsync(purchaseDetail);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PurchaseDetail purchaseDetail)
    {
        _context.PurchaseDetails.Update(purchaseDetail);
        await _context.SaveChangesAsync();
    }
}