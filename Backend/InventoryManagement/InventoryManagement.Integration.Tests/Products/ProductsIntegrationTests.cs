extern alias Web; // Alias para tu proyecto Web

using System.Net;
using System.Net.Http.Json;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Integration.Tests;

public class ProductsIntegrationTests : IntegrationTestBase
{
    public ProductsIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    // -----------------------------------------------------------------
    // PRUEBA 1: CREATE (Insertar)
    // -----------------------------------------------------------------
    [Theory]
    // 1. Happy Path (Datos correctos)
    [InlineData(true, 12345, "Laptop Pro", "Laptop de alta gama", 10)]
    
    // 2. Unhappy Path (Datos que violan las validaciones del DTO)
    [InlineData(false, 0, // SerialCode falla Range(1, ...)
                       "X", // Nombre falla MinimumLength = 3
                       "Desc...",
                       -5)] // TotalStock falla Range(0, ...)
    public async Task CreateProduct_Test(bool isHappyPath, short serialCode, string name, string description, short totalStock)
    {
        // Arrange
        var adminUser = await CreateTestUserAsync("Admin");
        var category = await CreateTestCategoryAsync(adminUser.Id);
        
        var newProductDto = new CreateProductDto
        {
            SerialCode = serialCode,
            Name = name,
            Description = description,
            CategoryId = category.Id, // Usamos una categoría válida en ambos casos
            TotalStock = totalStock
        };
        
        // En el Unhappy Path, si queremos que CategoryId también falle,
        // no podemos crear una categoría.
        if (!isHappyPath)
        {
            newProductDto.CategoryId = -1; // Hacemos que la Categoría también falle
        }

        // Act
        var response = await PostAsync("/api/products", newProductDto, adminUser.Id);

        // Assert
        if (isHappyPath)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var productFromResponse = await response.Content.ReadFromJsonAsync<Product>();
            Assert.NotNull(productFromResponse);

            var productFromDb = await DbContext.Products.FindAsync(productFromResponse.Id);
            Assert.NotNull(productFromDb);
            Assert.Equal(name, productFromDb.Name);
            Assert.Equal(serialCode, productFromDb.SerialCode);
        }
        else
        {
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    // -----------------------------------------------------------------
    // PRUEBA 2: READ (Select)
    // -----------------------------------------------------------------
    [Theory]
    // 1. Happy Path (El ID existirá)
    [InlineData(true)]
    // 2. Unhappy Path (El ID no existirá)
    [InlineData(false)]
    public async Task GetProductById_Test(bool isHappyPath)
    {
        // Arrange
        var adminUser = await CreateTestUserAsync("Admin");
        short productId;

        if (isHappyPath)
        {
            var category = await CreateTestCategoryAsync(adminUser.Id);
            var product = await CreateTestProductAsync(category.Id, adminUser.Id);
            productId = product.Id;
        }
        else
        {
            productId = 999;
        }

        // Act
        var response = await Client.GetAsync($"/api/products/{productId}");

        // Assert
        if (isHappyPath)
        {
            response.EnsureSuccessStatusCode();
            var product = await response.Content.ReadFromJsonAsync<Product>();
            Assert.NotNull(product);
            Assert.Equal(productId, product.Id);
        }
        else
        {
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    // -----------------------------------------------------------------
    // PRUEBA 3: UPDATE (Actualizar)
    // -----------------------------------------------------------------
    [Theory]
    // 1. Happy Path (Datos válidos)
    [InlineData(true, 5432, "Nombre Actualizado", "Desc Actualizada", 50)]
    
    // 2. Unhappy Path (Datos inválidos)
    [InlineData(false, -10, // SerialCode falla Range(1, ...)
                        "Y",  // Name falla MinimumLength = 3
                        "Desc...",
                        -100)] // TotalStock falla Range(0, ...)
    public async Task UpdateProduct_Test(bool isHappyPath, short serialCode, string name, string description, short totalStock)
    {
        // Arrange
        var adminUser = await CreateTestUserAsync("Admin");
        var category = await CreateTestCategoryAsync(adminUser.Id);
        var product = await CreateTestProductAsync(category.Id, adminUser.Id); // Producto original

        var updateDto = new CreateProductDto
        {
            SerialCode = serialCode,
            Name = name,
            Description = description,
            CategoryId = category.Id, // Usamos una categoría válida
            TotalStock = totalStock
        };

        // Act
        var response = await PutAsync($"/api/products/{product.Id}", updateDto);

        // Assert
        if (isHappyPath)
        {
            response.EnsureSuccessStatusCode();
            var productFromDb = await DbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.NotNull(productFromDb);
            Assert.Equal("Nombre Actualizado", productFromDb.Name);
            Assert.Equal(5432, productFromDb.SerialCode);
        }
        else
        {
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var productFromDb = await DbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.NotNull(productFromDb);
            Assert.Equal("Laptop Dell", productFromDb.Name); // Verifica que no se actualizó
        }
    }

    // -----------------------------------------------------------------
    // PRUEBA 4: DELETE (Borrar)
    // -----------------------------------------------------------------
    [Theory]
    // 1. Happy Path (Rol "Admin")
    [InlineData(true, "Admin")]
    // 2. Unhappy Path (Rol "Employee", sin permisos)
    [InlineData(false, "Employee")]
    public async Task DeleteProduct_Test(bool isHappyPath, string userRole)
    {
        // Arrange
        var adminUser = await CreateTestUserAsync("Admin");
        var category = await CreateTestCategoryAsync(adminUser.Id);
        var product = await CreateTestProductAsync(category.Id, adminUser.Id);
        Assert.Equal(1, product.Status);

        // Act
        var deleteResponse = await DeleteAsync($"/api/products/{product.Id}", userRole);

        // Assert
        if (isHappyPath)
        {
            deleteResponse.EnsureSuccessStatusCode();
            await DbContext.Entry(product).ReloadAsync();
            Assert.Equal(0, product.Status); // Borrado Lógico

            var getResponse = await Client.GetAsync($"/api/products/{product.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode); // No se debe poder encontrar
        }
        else
        {
            Assert.Equal(HttpStatusCode.BadRequest, deleteResponse.StatusCode);
            var error = await deleteResponse.Content.ReadAsStringAsync();
            Assert.Contains("Solo los administradores pueden eliminar", error);
            
            await DbContext.Entry(product).ReloadAsync();
            Assert.Equal(1, product.Status); // Verifica que no se borró
        }
    }
}