using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly InventoryDbContext _context;

    public PurchaseRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CreatePurchaseDto dto, short userId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var supplier = await _context.Suppliers.FindAsync(dto.SupplierId);
            if (supplier == null)
            {
                throw new InvalidOperationException("Proveedor no encontrado.");
            }

            var productIds = dto.PurchaseDetails.Select(pd => pd.ProductId).ToList();
            
            var productsFromDb = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            if (productsFromDb.Count != productIds.Count)
            {
                throw new InvalidOperationException("Uno o más productos no fueron encontrados.");
            }

            var purchase = new Purchase
            {
                SupplierId = dto.SupplierId,
                Supplier = supplier,
                CreatedByUserId = userId,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                Status = 1,
                PurchaseDetails = new List<PurchaseDetail>()
            };

            decimal total = 0;

            foreach (var detailDto in dto.PurchaseDetails)
            {
                var product = productsFromDb[detailDto.ProductId];

                var detail = new PurchaseDetail
                {
                    ProductId = detailDto.ProductId,
                    Quantity = detailDto.Quantity,
                    UnitPrice = detailDto.UnitPrice,
                    Purchase = purchase,
                    Product = product
                };
                
                purchase.PurchaseDetails.Add(detail);

                product.TotalStock += detailDto.Quantity;
                _context.Products.Update(product);

                total += detailDto.Quantity * detailDto.UnitPrice;
            }

            purchase.TotalPurchase = total;

            await _context.Purchases.AddAsync(purchase);
            
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Purchase>> GetAllAsync()
    {
        return await _context.Purchases
            .Include(p => p.Supplier)
            .Include(p => p.PurchaseDetails)
                .ThenInclude(pd => pd.Product)
            .ToListAsync();
    }

    public async Task<List<Purchase>> GetByUserIdAsync(short userId)
    {
        return await _context.Purchases
            .Where(p => p.CreatedByUserId == userId)
            .Include(p => p.Supplier)
            .Include(p => p.PurchaseDetails)
                .ThenInclude(pd => pd.Product)
            .ToListAsync();
    }

    public async Task<Purchase?> GetByIdAsync(int purchaseId)
    {
        return await _context.Purchases
            .Include(p => p.Supplier)
            .Include(p => p.PurchaseDetails)
                .ThenInclude(pd => pd.Product)
            .FirstOrDefaultAsync(p => p.Id == purchaseId);
    }

    public async Task UpdateAsync(Purchase purchase)
    {
        purchase.ModificationDate = DateTime.UtcNow;
        _context.Purchases.Update(purchase);
        await _context.SaveChangesAsync();
    }
}