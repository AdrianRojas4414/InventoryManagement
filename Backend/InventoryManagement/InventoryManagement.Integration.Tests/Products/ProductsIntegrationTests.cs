using System.Net;
using System.Net.Http.Json;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Integration.Tests.Products;

public class ProductsIntegrationTests : IntegrationTestBase
{
    public ProductsIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    #region CREATE Tests

    [Fact]
    public async Task CreateProduct_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);

        var createDto = new CreateProductDto
        {
            SerialCode = 12563,
            Name = "Laptop",
            Description = "New product description",
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

        // Verificar que se puede obtener por ID
        var getResponse = await Client.GetAsync($"/api/Products/{createdProduct.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var retrievedProduct = await getResponse.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(retrievedProduct);
        Assert.Equal(createdProduct.Id, retrievedProduct.Id);
        Assert.Equal(createDto.Name, retrievedProduct.Name);
    }

    [Fact]
    public async Task CreateProduct_WithDuplicateSerialCode_ShouldReturnBadRequest()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);
        var existingProduct = await CreateTestProductAsync(category.Id, user.Id);

        var createDto = new CreateProductDto
        {
            SerialCode = existingProduct.SerialCode, // C�digo duplicado
            Name = $"AnotherProduct_{Guid.NewGuid():N}",
            Description = "Description",
            CategoryId = category.Id,
            TotalStock = 10
        };

        // Act
        var response = await PostAsync("/api/Products", createDto, user.Id);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("c�digo serial", errorMessage.ToLower());
    }

    [Fact]
    public async Task CreateProduct_WithNonExistentCategory_ShouldReturnBadRequest()
    {
        // Arrange
        var user = await CreateTestUserAsync();

        var createDto = new CreateProductDto
        {
            SerialCode = (short)Random.Shared.Next(10000, 99999),
            Name = $"Product_{Guid.NewGuid():N}",
            Description = "Description",
            CategoryId = 9999, // Categor�a inexistente
            TotalStock = 10
        };

        // Act
        var response = await PostAsync("/api/Products", createDto, user.Id);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("categor�a", errorMessage.ToLower());
    }

    [Fact]
    public async Task CreateProduct_WithInactiveCategory_ShouldReturnBadRequest()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);
        category.Status = 0; // Inactivar categor�a
        DbContext.Categories.Update(category);
        await DbContext.SaveChangesAsync();

        var createDto = new CreateProductDto
        {
            SerialCode = (short)Random.Shared.Next(10000, 99999),
            Name = $"Product_{Guid.NewGuid():N}",
            Description = "Description",
            CategoryId = category.Id,
            TotalStock = 10
        };

        // Act
        var response = await PostAsync("/api/Products", createDto, user.Id);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("inactiva", errorMessage.ToLower());
    }

    #endregion

    #region READ Tests

    [Fact]
    public async Task GetAllProducts_ShouldReturnAllProducts()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);
        await CreateTestProductAsync(category.Id, user.Id);
        await CreateTestProductAsync(category.Id, user.Id);

        // Act
        var response = await Client.GetAsync("/api/Products");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        Assert.NotNull(products);
        Assert.True(products.Count >= 2);
    }

    [Fact]
    public async Task GetProductById_WithExistingId_ShouldReturnProduct()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);
        var product = await CreateTestProductAsync(category.Id, user.Id);

        // Act
        var response = await Client.GetAsync($"/api/Products/{product.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var retrievedProduct = await response.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(retrievedProduct);
        Assert.Equal(product.Id, retrievedProduct.Id);
        Assert.Equal(product.Name, retrievedProduct.Name);
    }

    [Fact]
    public async Task GetProductById_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var response = await Client.GetAsync("/api/Products/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_WithInactiveProduct_ShouldReturnNotFound()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);
        var product = await CreateTestProductAsync(category.Id, user.Id);
        product.Status = 0;
        DbContext.Products.Update(product);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/Products/{product.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region UPDATE Tests

    [Fact]
    public async Task UpdateProduct_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);
        var product = await CreateTestProductAsync(category.Id, user.Id);

        var updateDto = new CreateProductDto
        {
            SerialCode = (short)Random.Shared.Next(10000, 99999),
            Name = $"UpdatedProduct_{Guid.NewGuid():N}",
            Description = "Updated description",
            CategoryId = category.Id,
            TotalStock = 100
        };

        // Act
        var response = await PutAsync($"/api/Products/{product.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verificar que el producto se actualiz� correctamente
        var getResponse = await Client.GetAsync($"/api/Products/{product.Id}");
        var updatedProduct = await getResponse.Content.ReadFromJsonAsync<Product>();

        Assert.NotNull(updatedProduct);
        Assert.Equal(updateDto.Name, updatedProduct.Name);
        Assert.Equal(updateDto.Description, updatedProduct.Description);
        Assert.Equal(updateDto.TotalStock, updatedProduct.TotalStock);
    }

    [Fact]
    public async Task UpdateProduct_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);

        var updateDto = new CreateProductDto
        {
            SerialCode = (short)Random.Shared.Next(10000, 99999),
            Name = "Updated Product",
            Description = "Description",
            CategoryId = category.Id,
            TotalStock = 50
        };

        // Act
        var response = await PutAsync("/api/Products/99999", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_WithInactiveProduct_ShouldReturnNotFound()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);
        var product = await CreateTestProductAsync(category.Id, user.Id);
        product.Status = 0;
        DbContext.Products.Update(product);
        await DbContext.SaveChangesAsync();

        var updateDto = new CreateProductDto
        {
            SerialCode = (short)Random.Shared.Next(10000, 99999),
            Name = "Updated Product",
            Description = "Description",
            CategoryId = category.Id,
            TotalStock = 50
        };

        // Act
        var response = await PutAsync($"/api/Products/{product.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_WithDuplicateSerialCode_ShouldReturnBadRequest()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);
        var product1 = await CreateTestProductAsync(category.Id, user.Id);
        var product2 = await CreateTestProductAsync(category.Id, user.Id);

        var updateDto = new CreateProductDto
        {
            SerialCode = product1.SerialCode, // C�digo duplicado
            Name = "Updated Product",
            Description = "Description",
            CategoryId = category.Id,
            TotalStock = 50
        };

        // Act
        var response = await PutAsync($"/api/Products/{product2.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("c�digo serial", errorMessage.ToLower());
    }

    #endregion

    #region DELETE Tests

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

        // Verificar que el producto est� inactivo
        var getResponse = await Client.GetAsync($"/api/Products/{product.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        // Verificar en la base de datos que Status = 0
        var productInDb = await DbContext.Products.FindAsync(product.Id);
        Assert.NotNull(productInDb);
        Assert.Equal(0, productInDb.Status);
    }

    [Fact]
    public async Task DeleteProduct_AsNonAdmin_ShouldReturnBadRequest()
    {
        // Arrange
        var user = await CreateTestUserAsync("Employee");
        var category = await CreateTestCategoryAsync(user.Id);
        var product = await CreateTestProductAsync(category.Id, user.Id);

        // Act
        var response = await DeleteAsync($"/api/Products/{product.Id}", "Employee");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("administradores", errorMessage.ToLower());
    }

    [Fact]
    public async Task DeleteProduct_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var response = await DeleteAsync("/api/Products/99999", "Admin");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region ACTIVATE Tests

    [Fact]
    public async Task ActivateProduct_AsAdmin_ShouldActivateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync("Admin");
        var category = await CreateTestCategoryAsync(user.Id);
        var product = await CreateTestProductAsync(category.Id, user.Id);
        product.Status = 0;
        DbContext.Products.Update(product);
        await DbContext.SaveChangesAsync();

        // Act
        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/Products/{product.Id}/activate");
        request.Headers.Add("userRole", "Admin");
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verificar que el producto est� activo usando GetById
        var getResponse = await Client.GetAsync($"/api/Products/{product.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var activatedProduct = await getResponse.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(activatedProduct);
        Assert.Equal(1, activatedProduct.Status);
    }

    [Fact]
    public async Task ActivateProduct_AsNonAdmin_ShouldReturnBadRequest()
    {
        // Arrange
        var user = await CreateTestUserAsync("Employee");
        var category = await CreateTestCategoryAsync(user.Id);
        var product = await CreateTestProductAsync(category.Id, user.Id);
        product.Status = 0;
        DbContext.Products.Update(product);
        await DbContext.SaveChangesAsync();

        // Act
        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/Products/{product.Id}/activate");
        request.Headers.Add("userRole", "Employee");
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion
}