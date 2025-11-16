using System.Net;
using System.Net.Http.Json;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Integration.Tests.Purchases;

public class PurchasesIntegrationTests : IntegrationTestBase
{
    public PurchasesIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task CreatePurchase_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var supplier = await CreateTestSupplierAsync(user.Id);
        var category = await CreateTestCategoryAsync(user.Id);
        var product = await CreateTestProductAsync(category.Id, user.Id);

        var initialStock = product.TotalStock;

        var createDto = new CreatePurchaseDto
        {
            SupplierId = supplier.Id,
            PurchaseDetails = new List<CreatePurchaseDetailDto>
            {
                new CreatePurchaseDetailDto
                {
                    ProductId = product.Id,
                    Quantity = 10,
                    UnitPrice = 50.00m
                }
            }
        };

        // Act
        var response = await PostAsync("/api/Purchases", createDto, user.Id);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verificar que el stock se actualizó
        var updatedProduct = await DbContext.Products.FindAsync(product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equal(initialStock + 10, updatedProduct.TotalStock);
    }

    [Fact]
    public async Task GetAllPurchases_AsAdmin_ShouldReturnAllPurchases()
    {
        // Arrange
        var admin = await CreateTestUserAsync("Admin");
        var supplier = await CreateTestSupplierAsync(admin.Id);
        var category = await CreateTestCategoryAsync(admin.Id);
        var product = await CreateTestProductAsync(category.Id, admin.Id);

        var createDto = new CreatePurchaseDto
        {
            SupplierId = supplier.Id,
            PurchaseDetails = new List<CreatePurchaseDetailDto>
            {
                new CreatePurchaseDetailDto 
                { 
                    ProductId = product.Id, 
                    Quantity = 5, 
                    UnitPrice = 20.00m 
                }
            }
        };

        await PostAsync("/api/Purchases", createDto, admin.Id);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Purchases");
        request.Headers.Add("userRole", "Admin");
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var purchases = await response.Content.ReadFromJsonAsync<List<object>>();
        Assert.NotNull(purchases);
        Assert.NotEmpty(purchases);
    }

    [Fact]
    public async Task UpdatePurchase_AsAdmin_ShouldUpdateSuccessfully()
    {
        // Arrange
        var admin = await CreateTestUserAsync("Admin");
        var supplier = await CreateTestSupplierAsync(admin.Id);
        var category = await CreateTestCategoryAsync(admin.Id);
        var product = await CreateTestProductAsync(category.Id, admin.Id);

        var createDto = new CreatePurchaseDto
        {
            SupplierId = supplier.Id,
            PurchaseDetails = new List<CreatePurchaseDetailDto>
            {
                new CreatePurchaseDetailDto 
                { 
                    ProductId = product.Id, 
                    Quantity = 5, 
                    UnitPrice = 20.00m 
                }
            }
        };
        
        await PostAsync("/api/Purchases", createDto, admin.Id);

        var purchase = DbContext.Purchases.OrderByDescending(p => p.Id).First();

        // Act
        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/Purchases/{purchase.Id}");
        request.Headers.Add("userRole", "Admin");
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUserPurchases_ShouldReturnOnlyUserPurchases()
    {
        // Arrange
        var user = await CreateTestUserAsync("Employee");
        var supplier = await CreateTestSupplierAsync(user.Id);
        var category = await CreateTestCategoryAsync(user.Id);
        var product = await CreateTestProductAsync(category.Id, user.Id);

        var createDto = new CreatePurchaseDto
        {
            SupplierId = supplier.Id,
            PurchaseDetails = new List<CreatePurchaseDetailDto>
            {
                new CreatePurchaseDetailDto 
                { 
                    ProductId = product.Id, 
                    Quantity = 5, 
                    UnitPrice = 20.00m 
                }
            }
        };
        
        await PostAsync("/api/Purchases", createDto, user.Id);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Purchases/user");
        request.Headers.Add("userId", user.Id.ToString());
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var purchases = await response.Content.ReadFromJsonAsync<List<PurchaseResponseDto>>();
        Assert.NotNull(purchases);
        Assert.NotEmpty(purchases);
    }
}