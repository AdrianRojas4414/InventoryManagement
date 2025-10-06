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

    // Registrar nueva compra junto con sus detalles
    public async Task AddAsync(CreatePurchaseDto dto, short userId)
    {
        // Obtener el proveedor correspondiente
        var supplier = await _context.Suppliers.FindAsync(dto.SupplierId);
        if (supplier == null)
            throw new InvalidOperationException("Proveedor no encontrado.");

        // Crear la entidad Purchase
        var purchase = new Purchase
        {
            SupplierId = dto.SupplierId,
            Supplier = supplier,
            UserId = userId,
            // Inicializar PurchaseDetails asignando la referencia a purchase después
            PurchaseDetails = new List<PurchaseDetail>()
        };

        // Crear los detalles y asignar la referencia a purchase
        foreach (var pd in dto.PurchaseDetails)
        {
            var product = await _context.Products.FindAsync(pd.ProductId);
            if (product == null)
                throw new InvalidOperationException("Producto no encontrado.");

            var detail = new PurchaseDetail
            {
                ProductId = pd.ProductId,
                Quantity = pd.Quantity,
                UnitPrice = pd.UnitPrice,
                Purchase = purchase, // Asignar la referencia requerida
                Product = product    // Asignar la referencia requerida
            };
            purchase.PurchaseDetails.Add(detail);
        }

        // Calcular el total automáticamente
        purchase.TotalPurchase = purchase.PurchaseDetails.Sum(pd => pd.UnitPrice * pd.Quantity);

        // Guardar la compra
        await _context.Purchases.AddAsync(purchase);
        await _context.SaveChangesAsync();

        // Actualizar stock de cada producto
        foreach (var detail in purchase.PurchaseDetails)
        {
            var product = await _context.Products.FindAsync(detail.ProductId);
            if (product != null)
            {
                product.TotalStock += detail.Quantity;
            }
        }

        await _context.SaveChangesAsync();
    }

    // Listar todas las compras (solo admin)
    public async Task<List<Purchase>> GetAllAsync()
    {
        return await _context.Purchases
            .Include(p => p.Supplier)
            .Include(p => p.PurchaseDetails)
                .ThenInclude(pd => pd.Product)
            .ToListAsync();
    }

    // Listar compras de un usuario específico (empleado)
    public async Task<List<Purchase>> GetByUserIdAsync(short userId)
    {
        return await _context.Purchases
            .Where(p => p.UserId == userId)
            .Include(p => p.Supplier)
            .Include(p => p.PurchaseDetails)
                .ThenInclude(pd => pd.Product)
            .ToListAsync();
    }

    // Obtener una compra por Id
    public async Task<Purchase?> GetByIdAsync(int purchaseId)
    {
        return await _context.Purchases
            .Include(p => p.Supplier)
            .Include(p => p.PurchaseDetails)
                .ThenInclude(pd => pd.Product)
            .FirstOrDefaultAsync(p => p.Id == purchaseId);
    }

    // Actualizar compra (solo admin)
    public async Task UpdateAsync(Purchase purchase)
    {
        // Recalcular TotalPurchase
        purchase.TotalPurchase = purchase.PurchaseDetails.Sum(pd => pd.UnitPrice * pd.Quantity);

        // Actualizar stock
        foreach (var detail in purchase.PurchaseDetails)
        {
            var product = await _context.Products.FindAsync(detail.ProductId);
            if (product != null)
            {
                product.TotalStock += detail.Quantity;
            }
        }

        _context.Purchases.Update(purchase);
        await _context.SaveChangesAsync();
    }
}