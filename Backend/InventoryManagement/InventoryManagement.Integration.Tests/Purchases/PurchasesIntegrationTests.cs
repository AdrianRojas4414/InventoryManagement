extern alias Web; // Alias para tu proyecto Web

using System.Net;
using System.Net.Http.Json;
using System.Collections.Generic;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Integration.Tests;

public class PurchasesIntegrationTests : IntegrationTestBase
{
    public PurchasesIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    // -----------------------------------------------------------------
    // PRUEBA 1: CREATE (La Compra)
    // -----------------------------------------------------------------
    [Theory]
    // 1. Happy Path (Compra válida)
    [InlineData(true)]
    
    // 2. Unhappy Path (IDs de Proveedor/Producto inválidos)
    [InlineData(false)]
    public async Task CreatePurchase_Test(bool isHappyPath)
    {
        // Arrange
        // (Crear usuario, proveedor, categoría y productos)
        var adminUser = await CreateTestUserAsync("Admin");
        var supplier = await CreateTestSupplierAsync(adminUser.Id);
        var category = await CreateTestCategoryAsync(adminUser.Id);

        // Creamos el Producto 1 (usando el helper)
        var product1 = await CreateTestProductAsync(category.Id, adminUser.Id); // Stock inicial: 10
        
        // Creamos el Producto 2 (manualmente, para evitar violación de UNIQUE constraint del helper)
        var product2 = new Product
        {
            SerialCode = 1002, // Serial único
            Name = "Teclado Mecánico", // Nombre único
            CategoryId = category.Id,
            TotalStock = 20, // Stock inicial: 20
            Status = 1,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
            CreatedByUserId = adminUser.Id
        };
        DbContext.Products.Add(product2);
        await DbContext.SaveChangesAsync();

        
        CreatePurchaseDto purchaseDto;

        if (isHappyPath)
        {
            // --- ARRANGE (HAPPY PATH) ---
            purchaseDto = new CreatePurchaseDto
            {
                SupplierId = supplier.Id, // ID Válido
                PurchaseDetails = new List<CreatePurchaseDetailDto>
                {
                    // Comprar 5 del Producto 1
                    new() { ProductId = product1.Id, Quantity = 5, UnitPrice = 100.00m }, 
                    // Comprar 10 del Producto 2
                    new() { ProductId = product2.Id, Quantity = 10, UnitPrice = 50.00m }  
                }
            };
        }
        else
        {
            // --- ARRANGE (UNHAPPY PATH) ---
            // Enviamos atributos que fallarán la lógica del repositorio
            purchaseDto = new CreatePurchaseDto
            {
                SupplierId = 999, // ID de Proveedor Inválido
                PurchaseDetails = new List<CreatePurchaseDetailDto>
                {
                    // Producto 1 existe, pero Producto 2 (ID 888) no.
                    new() { ProductId = product1.Id, Quantity = 5, UnitPrice = 100.00m },
                    new() { ProductId = 888, Quantity = 10, UnitPrice = 50.00m }
                }
            };
        }

        // Act
        var response = await PostAsync("/api/purchases", purchaseDto, adminUser.Id);

        // Assert
        if (isHappyPath)
        {
            // 1. Verificar Respuesta OK
            response.EnsureSuccessStatusCode(); // Se espera 200 OK
            
            // 2. Verificar que la Compra se guardó en la DB
            var purchase = await DbContext.Purchases
                .Include(p => p.PurchaseDetails)
                .FirstOrDefaultAsync(p => p.SupplierId == supplier.Id);
            
            Assert.NotNull(purchase);
            
            // 3. Verificar el Total ( (5 * 100) + (10 * 50) = 500 + 500 = 1000 )
            Assert.Equal(1000.00m, purchase.TotalPurchase);
            Assert.Equal(2, purchase.PurchaseDetails.Count);

            // 4. VERIFICAR ACTUALIZACIÓN DE STOCK (¡El paso más importante!)
            await DbContext.Entry(product1).ReloadAsync();
            await DbContext.Entry(product2).ReloadAsync();

            Assert.Equal(10 + 5, product1.TotalStock); // Stock inicial 10 + 5 comprados
            Assert.Equal(20 + 10, product2.TotalStock); // Stock inicial 20 + 10 comprados
        }
        else
        {
            // 1. Verificar Respuesta BadRequest
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // 2. Verificar Mensaje de Error
            // El repositorio debe fallar primero al no encontrar el Proveedor 999
            var error = await response.Content.ReadFromJsonAsync<object>(); 
            Assert.Contains("Proveedor no encontrado", error.ToString()!);

            // 3. Verificar que NO se creó la compra
            var purchaseCount = await DbContext.Purchases.CountAsync();
            Assert.Equal(0, purchaseCount);
            
            // 4. Verificar que NO se actualizó el stock
            await DbContext.Entry(product1).ReloadAsync();
            Assert.Equal(10, product1.TotalStock); // Sigue en su stock inicial
        }
    }
}