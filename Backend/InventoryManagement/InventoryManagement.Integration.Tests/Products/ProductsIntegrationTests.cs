using System.Net;
using System.Net.Http.Json;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Integration.Tests.Products;

public class ProductsIntegrationTests : IntegrationTestBase
{
    public ProductsIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateProduct_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);

        var createDto = new CreateProductDto
        {
            SerialCode = 20012,
            Name = "Mouse Logitech",
            Description = "Mouse inalámbrico",
            CategoryId = category.Id,
            TotalStock = 50
        };

        // Act
        var response = await PostAsync("/api/Products", createDto, user.Id);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var createdProduct = await response.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(createdProduct);
        Assert.Equal(createDto.Name, createdProduct.Name);
        Assert.Equal(createDto.SerialCode, createdProduct.SerialCode);
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnAllProducts()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);
        await CreateTestProductAsync(category.Id, user.Id);

        // Act
        var response = await Client.GetAsync("/api/Products");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        Assert.NotNull(products);
        Assert.NotEmpty(products);
    }

    [Fact]
    public async Task UpdateProduct_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);
        var product = await CreateTestProductAsync(category.Id, user.Id);

        var updateDto = new CreateProductDto
        {
            SerialCode = 2002,
            Name = "Laptop HP Actualizada",
            Description = "Descripción actualizada",
            CategoryId = category.Id,
            TotalStock = 100
        };

        // Act
        var response = await PutAsync($"/api/Products/{product.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var getResponse = await Client.GetAsync($"/api/Products/{product.Id}");
        var updatedProduct = await getResponse.Content.ReadFromJsonAsync<Product>();

        Assert.NotNull(updatedProduct);
        Assert.Equal(updateDto.Name, updatedProduct.Name);
        Assert.Equal(updateDto.TotalStock, updatedProduct.TotalStock);
    }

    [Fact]
    public async Task DeleteProduct_AsAdmin_ShouldDeactivateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync("Admin");
        var category = await CreateTestCategoryAsync(user.Id);
        var product = await CreateTestProductAsync(category.Id, user.Id);

        // Act
        var response = await DeleteAsync($"/api/Products/{product.Id}", "Admin");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verificar que está inactivo
        var productInDb = await DbContext.Products.FindAsync(product.Id);
        Assert.NotNull(productInDb);
        Assert.Equal(0, productInDb.Status);
    }
}