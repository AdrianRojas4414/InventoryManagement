extern alias Web;

using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

// 2. Usar "Web::" para TODOS los 'using' de tu proyecto
using Web::InventoryManagement.Application.DTOs;
using Web::InventoryManagement.Application.Interfaces;
using Web::InventoryManagement.Infrastructure.Repositories;
using Web::InventoryManagement.Controllers;

namespace InventoryManagement.IntegrationTests;

public class PurchasesControllerTests : TestBase
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly PurchasesController _controller;

    public PurchasesControllerTests()
    {
        // NOTA: Si tu 'PurchaseRepository' real necesita OTROS repositorios
        // en su constructor (ej. IProductRepository), deberás instanciarlos aquí
        // usando 'new ProductRepository(DbContext)', etc.
        // Por ahora, asumimos que solo necesita el DbContext.
        _purchaseRepository = new PurchaseRepository(DbContext); 
        
        _controller = new PurchasesController(_purchaseRepository);
    }

    // Cubre: Proceso Principal (Comprar un producto - Éxito)
    [Fact]
    public async Task CreatePurchase_WithValidData_ShouldSucceedAndIncreaseStock()
    {
        // --- ARRANGE ---
        // 1. Obtenemos el stock inicial.
        // (Usamos los IDs sembrados en TestBase para probar la integración)
        var product = await DbContext.Products.FindAsync(DefaultProductId);
        var stockInicial = product!.TotalStock; // Es 50
        
        var dto = new CreatePurchaseDto
        {
            SupplierId = DefaultSupplierId,
            PurchaseDetails = new List<CreatePurchaseDetailDto>
            {
                new() { ProductId = DefaultProductId, Quantity = 10, UnitPrice = 15.50m }
            }
        };

        // --- ACT ---
        // 2. Ejecutamos la compra
        var actionResult = await _controller.CreatePurchase(dto, EmployeeUserId);
        
        // --- ASSERT (API) ---
        // 3. Verificamos que la API respondió OK
        Assert.IsType<OkObjectResult>(actionResult);

        // --- ASSERT (INTEGRACIÓN Y PERSISTENCIA) ---
        
        // 4. Verificamos que el STOCK del producto aumentó
        // Esta es la prueba de integración más importante.
        var productActualizado = await DbContext.Products.FindAsync(DefaultProductId);
        Assert.Equal(stockInicial + 10, productActualizado!.TotalStock); // Debe ser 60

        // 5. Verificamos que la COMPRA y el DETALLE se crearon
        var purchase = await DbContext.Purchases
            .Include(p => p.PurchaseDetails)
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync();
        
        Assert.NotNull(purchase);
        Assert.Equal(DefaultSupplierId, purchase.SupplierId);
        Assert.Equal(EmployeeUserId, purchase.CreatedByUserId);
        Assert.Equal(155.00m, purchase.TotalPurchase); // 10 * 15.50
        Assert.Equal(10, purchase.PurchaseDetails.First().Quantity);
    }

    // Cubre: Proceso Principal (Compra sin éxito)
    [Theory]
    [InlineData(999, DefaultProductId)] // Proveedor no existe
    [InlineData(DefaultSupplierId, 999)] // Producto no existe
    public async Task CreatePurchase_WithInvalidIds_ShouldReturnBadRequest(short supplierId, short productId)
    {
        // Arrange
        var dto = new CreatePurchaseDto
        {
            SupplierId = supplierId,
            PurchaseDetails = new List<CreatePurchaseDetailDto>
            {
                new() { ProductId = productId, Quantity = 5, UnitPrice = 10m }
            }
        };

        // Act
        // Tu controlador captura InvalidOperationException y devuelve BadRequest
        var actionResult = await _controller.CreatePurchase(dto, EmployeeUserId);

        // Assert
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }
}