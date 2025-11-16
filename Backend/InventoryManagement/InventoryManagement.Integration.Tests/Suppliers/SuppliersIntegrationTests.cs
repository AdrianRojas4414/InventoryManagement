using System.Net;
using System.Net.Http.Json;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Integration.Tests.Suppliers;

public class SuppliersIntegrationTests : IntegrationTestBase
{
    public SuppliersIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    #region CREATE Tests

    [Fact]
    public async Task CreateSupplier_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync();

        var createDto = new CreateSupplierDto
        {
            Name = $"NewSupplier_{Guid.NewGuid():N}",
            Nit = $"{Random.Shared.Next(1000000, 9999999)}",
            Address = "123 Test Street",
            Phone = "12345678",
            Email = $"newsupplier_{Guid.NewGuid():N}@test.com",
            ContactName = "John Doe"
        };

        // Act
        var response = await PostAsync("/api/Suppliers", createDto, user.Id);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdSupplier = await response.Content.ReadFromJsonAsync<Supplier>();
        Assert.NotNull(createdSupplier);
        Assert.Equal(createDto.Name, createdSupplier.Name);
        Assert.Equal(createDto.Nit, createdSupplier.Nit);

        // Verificar que se puede obtener por ID
        var getResponse = await Client.GetAsync($"/api/Suppliers/{createdSupplier.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var retrievedSupplier = await getResponse.Content.ReadFromJsonAsync<Supplier>();
        Assert.NotNull(retrievedSupplier);
        Assert.Equal(createdSupplier.Id, retrievedSupplier.Id);
        Assert.Equal(createDto.Name, retrievedSupplier.Name);
    }

    [Fact]
    public async Task CreateSupplier_WithDuplicateNit_ShouldReturnBadRequest()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var existingSupplier = await CreateTestSupplierAsync(user.Id);

        var createDto = new CreateSupplierDto
        {
            Name = $"AnotherSupplier_{Guid.NewGuid():N}",
            Nit = existingSupplier.Nit, // NIT duplicado
            Address = "456 Test Avenue",
            Phone = "87654321",
            Email = $"another_{Guid.NewGuid():N}@test.com",
            ContactName = "Jane Smith"
        };

        // Act
        var response = await PostAsync("/api/Suppliers", createDto, user.Id);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("nit", errorMessage.ToLower());
    }

    [Fact]
    public async Task CreateSupplier_WithDuplicateEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var existingSupplier = await CreateTestSupplierAsync(user.Id);

        var createDto = new CreateSupplierDto
        {
            Name = $"AnotherSupplier_{Guid.NewGuid():N}",
            Nit = $"{Random.Shared.Next(1000000, 9999999)}",
            Address = "456 Test Avenue",
            Phone = "87654321",
            Email = existingSupplier.Email, // Email duplicado
            ContactName = "Jane Smith"
        };

        // Act
        var response = await PostAsync("/api/Suppliers", createDto, user.Id);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("correo", errorMessage.ToLower());
    }

    #endregion

    #region READ Tests

    [Fact]
    public async Task GetAllSuppliers_ShouldReturnAllSuppliers()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        await CreateTestSupplierAsync(user.Id);
        await CreateTestSupplierAsync(user.Id);

        // Act
        var response = await Client.GetAsync("/api/Suppliers");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var suppliers = await response.Content.ReadFromJsonAsync<List<Supplier>>();
        Assert.NotNull(suppliers);
        Assert.True(suppliers.Count >= 2);
    }

    [Fact]
    public async Task GetSupplierById_WithExistingId_ShouldReturnSupplier()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var supplier = await CreateTestSupplierAsync(user.Id);

        // Act
        var response = await Client.GetAsync($"/api/Suppliers/{supplier.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var retrievedSupplier = await response.Content.ReadFromJsonAsync<Supplier>();
        Assert.NotNull(retrievedSupplier);
        Assert.Equal(supplier.Id, retrievedSupplier.Id);
        Assert.Equal(supplier.Name, retrievedSupplier.Name);
    }

    [Fact]
    public async Task GetSupplierById_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var response = await Client.GetAsync("/api/Suppliers/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetSupplierById_WithInactiveSupplier_ShouldReturnNotFound()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var supplier = await CreateTestSupplierAsync(user.Id);
        supplier.Status = 0;
        DbContext.Suppliers.Update(supplier);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/Suppliers/{supplier.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region UPDATE Tests

    [Fact]
    public async Task UpdateSupplier_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var supplier = await CreateTestSupplierAsync(user.Id);

        var updateDto = new UpdateSupplierDto
        {
            Name = $"UpdatedSupplier_{Guid.NewGuid():N}",
            Nit = $"{Random.Shared.Next(1000000, 9999999)}",
            Address = "Updated Address",
            Phone = "99999999",
            Email = $"updated_{Guid.NewGuid():N}@test.com",
            ContactName = "Updated Contact"
        };

        // Act
        var response = await PutAsync($"/api/Suppliers/{supplier.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verificar que el proveedor se actualizó correctamente
        var getResponse = await Client.GetAsync($"/api/Suppliers/{supplier.Id}");
        var updatedSupplier = await getResponse.Content.ReadFromJsonAsync<Supplier>();

        Assert.NotNull(updatedSupplier);
        Assert.Equal(updateDto.Name, updatedSupplier.Name);
        Assert.Equal(updateDto.Nit, updatedSupplier.Nit);
        Assert.Equal(updateDto.Address, updatedSupplier.Address);
        Assert.Equal(updateDto.Email, updatedSupplier.Email);
    }

    [Fact]
    public async Task UpdateSupplier_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var updateDto = new UpdateSupplierDto
        {
            Name = "Updated Supplier",
            Nit = "1234567",
            Address = "Address",
            Phone = "12345678",
            Email = "test@test.com",
            ContactName = "Contact"
        };

        // Act
        var response = await PutAsync("/api/Suppliers/99999", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSupplier_WithInactiveSupplier_ShouldReturnNotFound()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var supplier = await CreateTestSupplierAsync(user.Id);
        supplier.Status = 0;
        DbContext.Suppliers.Update(supplier);
        await DbContext.SaveChangesAsync();

        var updateDto = new UpdateSupplierDto
        {
            Name = "Updated Supplier",
            Nit = "1234567",
            Address = "Address",
            Phone = "12345678",
            Email = "test@test.com",
            ContactName = "Contact"
        };

        // Act
        var response = await PutAsync($"/api/Suppliers/{supplier.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSupplier_WithDuplicateNit_ShouldReturnBadRequest()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var supplier1 = await CreateTestSupplierAsync(user.Id);
        var supplier2 = await CreateTestSupplierAsync(user.Id);

        var updateDto = new UpdateSupplierDto
        {
            Name = "Updated Supplier",
            Nit = supplier1.Nit, // NIT duplicado
            Address = "Address",
            Phone = "12345678",
            Email = $"unique_{Guid.NewGuid():N}@test.com",
            ContactName = "Contact"
        };

        // Act
        var response = await PutAsync($"/api/Suppliers/{supplier2.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("nit", errorMessage.ToLower());
    }

    [Fact]
    public async Task UpdateSupplier_WithDuplicateEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var supplier1 = await CreateTestSupplierAsync(user.Id);
        var supplier2 = await CreateTestSupplierAsync(user.Id);

        var updateDto = new UpdateSupplierDto
        {
            Name = "Updated Supplier",
            Nit = $"{Random.Shared.Next(1000000, 9999999)}",
            Address = "Address",
            Phone = "12345678",
            Email = supplier1.Email, // Email duplicado
            ContactName = "Contact"
        };

        // Act
        var response = await PutAsync($"/api/Suppliers/{supplier2.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("correo", errorMessage.ToLower());
    }

    #endregion

    #region DELETE Tests

    [Fact]
    public async Task DeactivateSupplier_AsAdmin_ShouldDeactivateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync("Admin");
        var supplier = await CreateTestSupplierAsync(user.Id);

        // Act
        var response = await DeleteAsync($"/api/Suppliers/{supplier.Id}", "Admin");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verificar que el proveedor está inactivo
        var getResponse = await Client.GetAsync($"/api/Suppliers/{supplier.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        // Verificar en la base de datos que Status = 0
        var supplierInDb = await DbContext.Suppliers.FindAsync(supplier.Id);
        Assert.NotNull(supplierInDb);
        Assert.Equal(0, supplierInDb.Status);
    }

    [Fact]
    public async Task DeactivateSupplier_AsNonAdmin_ShouldReturnBadRequest()
    {
        // Arrange
        var user = await CreateTestUserAsync("Employee");
        var supplier = await CreateTestSupplierAsync(user.Id);

        // Act
        var response = await DeleteAsync($"/api/Suppliers/{supplier.Id}", "Employee");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("administradores", errorMessage.ToLower());
    }

    [Fact]
    public async Task DeactivateSupplier_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var response = await DeleteAsync("/api/Suppliers/99999", "Admin");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region ACTIVATE Tests

    [Fact]
    public async Task ActivateSupplier_AsAdmin_ShouldActivateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync("Admin");
        var supplier = await CreateTestSupplierAsync(user.Id);
        supplier.Status = 0;
        DbContext.Suppliers.Update(supplier);
        await DbContext.SaveChangesAsync();

        // Act
        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/Suppliers/{supplier.Id}/activate");
        request.Headers.Add("userRole", "Admin");
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verificar que el proveedor está activo usando GetById
        var getResponse = await Client.GetAsync($"/api/Suppliers/{supplier.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var activatedSupplier = await getResponse.Content.ReadFromJsonAsync<Supplier>();
        Assert.NotNull(activatedSupplier);
        Assert.Equal(1, activatedSupplier.Status);
    }

    [Fact]
    public async Task ActivateSupplier_AsNonAdmin_ShouldReturnBadRequest()
    {
        // Arrange
        var user = await CreateTestUserAsync("Employee");
        var supplier = await CreateTestSupplierAsync(user.Id);
        supplier.Status = 0;
        DbContext.Suppliers.Update(supplier);
        await DbContext.SaveChangesAsync();

        // Act
        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/Suppliers/{supplier.Id}/activate");
        request.Headers.Add("userRole", "Employee");
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ActivateSupplier_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/Suppliers/99999/activate");
        request.Headers.Add("userRole", "Admin");
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion
}