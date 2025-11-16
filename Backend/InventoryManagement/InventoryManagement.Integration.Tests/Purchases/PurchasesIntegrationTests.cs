using System.Net;
using System.Net.Http.Json;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Integration.Tests.Purchases;

public class PurchasesIntegrationTests : IntegrationTestBase
{
    public PurchasesIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    #region CREATE Tests

    [Fact]
    public async Task CreatePurchase_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var supplier = await CreateTestSupplierAsync(user.Id);
        var category = await CreateTestCategoryAsync(user.Id);
        var product1 = await CreateTestProductAsync(category.Id, user.Id);
        var product2 = await CreateTestProductAsync(category.Id, user.Id);

        var initialStock1 = product1.TotalStock;
        var initialStock2 = product2.TotalStock;

        var createDto = new CreatePurchaseDto
        {
            SupplierId = supplier.Id,
            PurchaseDetails = new List<CreatePurchaseDetailDto>
            {
                new CreatePurchaseDetailDto
                {
                    ProductId = product1.Id,
                    Quantity = 10,
                    UnitPrice = 50.00m
                },
                new CreatePurchaseDetailDto
                {
                    ProductId = product2.Id,
                    Quantity = 5,
                    UnitPrice = 30.00m
                }
            }
        };

        // Act
        var response = await PostAsync("/api/Purchases", createDto, user.Id);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Obtener todas las compras del usuario
        var getUserPurchasesResponse = await Client.GetAsync("/api/Purchases/user");
        getUserPurchasesResponse.Headers.Add("userId", user.Id.ToString());

        var purchases = await getUserPurchasesResponse.Content.ReadFromJsonAsync<List<PurchaseResponseDto>>();
        Assert.NotNull(purchases);
        Assert.NotEmpty(purchases);

        // Encontrar la compra reci�n creada (la �ltima)
        var createdPurchase = purchases.OrderByDescending(p => p.Id).First();
        Assert.Equal(supplier.Name, createdPurchase.Proveedor);

        // Verificar que el stock de los productos se actualiz�
        var updatedProduct1 = await DbContext.Products.FindAsync(product1.Id);
        var updatedProduct2 = await DbContext.Products.FindAsync(product2.Id);

        Assert.NotNull(updatedProduct1);
        Assert.NotNull(updatedProduct2);
        Assert.Equal(initialStock1 + 10, updatedProduct1.TotalStock);
        Assert.Equal(initialStock2 + 5, updatedProduct2.TotalStock);
    }

    [Fact]
    public async Task CreatePurchase_WithNonExistentSupplier_ShouldReturnBadRequest()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);
        var product = await CreateTestProductAsync(category.Id, user.Id);

        var createDto = new CreatePurchaseDto
        {
            SupplierId = 9999, // Proveedor inexistente
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
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("proveedor", errorMessage.ToLower());
    }

    [Fact]
    public async Task CreatePurchase_WithNonExistentProduct_ShouldReturnBadRequest()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var supplier = await CreateTestSupplierAsync(user.Id);

        var createDto = new CreatePurchaseDto
        {
            SupplierId = supplier.Id,
            PurchaseDetails = new List<CreatePurchaseDetailDto>
            {
                new CreatePurchaseDetailDto
                {
                    ProductId = 9999, // Producto inexistente
                    Quantity = 10,
                    UnitPrice = 50.00m
                }
            }
        };

        // Act
        var response = await PostAsync("/api/Purchases", createDto, user.Id);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("productos", errorMessage.ToLower());
    }

    [Fact]
    public async Task CreatePurchase_WithEmptyDetails_ShouldCreatePurchaseWithZeroTotal()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var supplier = await CreateTestSupplierAsync(user.Id);

        var createDto = new CreatePurchaseDto
        {
            SupplierId = supplier.Id,
            PurchaseDetails = new List<CreatePurchaseDetailDto>()
        };

        // Act
        var response = await PostAsync("/api/Purchases", createDto, user.Id);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region READ Tests

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
                new CreatePurchaseDetailDto { ProductId = product.Id, Quantity = 5, UnitPrice = 20.00m }
            }
        };

        await PostAsync("/api/Purchases", createDto, admin.Id);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Purchases");
        request.Headers.Add("userRole", "Admin");
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var purchases = await response.Content.ReadFromJsonAsync<List<Purchase>>();
        Assert.NotNull(purchases);
        Assert.NotEmpty(purchases);
    }

    [Fact]
    public async Task GetAllPurchases_AsNonAdmin_ShouldReturnForbidden()
    {
        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Purchases");
        request.Headers.Add("userRole", "Employee");
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetUserPurchases_ShouldReturnOnlyUserPurchases()
    {
        // Arrange
        var user1 = await CreateTestUserAsync();
        var user2 = await CreateTestUserAsync();
        var supplier = await CreateTestSupplierAsync(user1.Id);
        var category = await CreateTestCategoryAsync(user1.Id);
        var product = await CreateTestProductAsync(category.Id, user1.Id);

        // Crear compra para user1
        var createDto1 = new CreatePurchaseDto
        {
            SupplierId = supplier.Id,
            PurchaseDetails = new List<CreatePurchaseDetailDto>
            {
                new CreatePurchaseDetailDto { ProductId = product.Id, Quantity = 5, UnitPrice = 20.00m }
            }
        };
        await PostAsync("/api/Purchases", createDto1, user1.Id);

        // Crear compra para user2
        var createDto2 = new CreatePurchaseDto
        {
            SupplierId = supplier.Id,
            PurchaseDetails = new List<CreatePurchaseDetailDto>
            {
                new CreatePurchaseDetailDto { ProductId = product.Id, Quantity = 3, UnitPrice = 15.00m }
            }
        };
        await PostAsync("/api/Purchases", createDto2, user2.Id);

        // Act - Obtener compras de user1
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Purchases/user");
        request.Headers.Add("userId", user1.Id.ToString());
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var purchases = await response.Content.ReadFromJsonAsync<List<PurchaseResponseDto>>();
        Assert.NotNull(purchases);

        // Verificar que todas las compras son del user1
        var purchasesInDb = await DbContext.Purchases
            .Where(p => p.CreatedByUserId == user1.Id)
            .ToListAsync();
        Assert.NotEmpty(purchasesInDb);
    }

    [Fact]
    public async Task GetPurchaseById_AsOwner_ShouldReturnPurchase()
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
                new CreatePurchaseDetailDto { ProductId = product.Id, Quantity = 5, UnitPrice = 20.00m }
            }
        };
        await PostAsync("/api/Purchases", createDto, user.Id);

        // Obtener el ID de la compra creada
        var purchase = await DbContext.Purchases
            .Where(p => p.CreatedByUserId == user.Id)
            .OrderByDescending(p => p.Id)
            .FirstAsync();

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Purchases/{purchase.Id}");
        request.Headers.Add("userId", user.Id.ToString());
        request.Headers.Add("userRole", "Employee");
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var retrievedPurchase = await response.Content.ReadFromJsonAsync<PurchaseResponseDto>();
        Assert.NotNull(retrievedPurchase);
        Assert.Equal(purchase.Id, retrievedPurchase.Id);
    }

    [Fact]
    public async Task GetPurchaseById_AsNonOwnerNonAdmin_ShouldReturnForbidden()
    {
        // Arrange
        var user1 = await CreateTestUserAsync("Employee");
        var user2 = await CreateTestUserAsync("Employee");
        var supplier = await CreateTestSupplierAsync(user1.Id);
        var category = await CreateTestCategoryAsync(user1.Id);
        var product = await CreateTestProductAsync(category.Id, user1.Id);

        var createDto = new CreatePurchaseDto
        {
            SupplierId = supplier.Id,
            PurchaseDetails = new List<CreatePurchaseDetailDto>
            {
                new CreatePurchaseDetailDto { ProductId = product.Id, Quantity = 5, UnitPrice = 20.00m }
            }
        };
        await PostAsync("/api/Purchases", createDto, user1.Id);

        var purchase = await DbContext.Purchases
            .Where(p => p.CreatedByUserId == user1.Id)
            .OrderByDescending(p => p.Id)
            .FirstAsync();

        // Act - user2 intenta acceder a la compra de user1
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Purchases/{purchase.Id}");
        request.Headers.Add("userId", user2.Id.ToString());
        request.Headers.Add("userRole", "Employee");
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetPurchaseById_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var user = await CreateTestUserAsync("Admin");

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Purchases/99999");
        request.Headers.Add("userId", user.Id.ToString());
        request.Headers.Add("userRole", "Admin");
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region UPDATE Tests

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
                new CreatePurchaseDetailDto { ProductId = product.Id, Quantity = 5, UnitPrice = 20.00m }
            }
        };
        await PostAsync("/api/Purchases", createDto, admin.Id);

        var purchase = await DbContext.Purchases
            .Where(p => p.CreatedByUserId == admin.Id)
            .OrderByDescending(p => p.Id)
            .FirstAsync();

        var oldModificationDate = purchase.ModificationDate;
        await Task.Delay(1000); // Esperar 1 segundo para asegurar cambio en fecha

        // Act
        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/Purchases/{purchase.Id}");
        request.Headers.Add("userRole", "Admin");
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verificar que se actualiz� la fecha de modificaci�n
        await DbContext.Entry(purchase).ReloadAsync();
        Assert.True(purchase.ModificationDate > oldModificationDate);
    }

    [Fact]
    public async Task UpdatePurchase_AsNonAdmin_ShouldReturnForbidden()
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
                new CreatePurchaseDetailDto { ProductId = product.Id, Quantity = 5, UnitPrice = 20.00m }
            }
        };
        await PostAsync("/api/Purchases", createDto, user.Id);

        var purchase = await DbContext.Purchases
            .Where(p => p.CreatedByUserId == user.Id)
            .OrderByDescending(p => p.Id)
            .FirstAsync();

        // Act
        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/Purchases/{purchase.Id}");
        request.Headers.Add("userRole", "Employee");
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePurchase_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/Purchases/99999");
        request.Headers.Add("userRole", "Admin");
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion
}