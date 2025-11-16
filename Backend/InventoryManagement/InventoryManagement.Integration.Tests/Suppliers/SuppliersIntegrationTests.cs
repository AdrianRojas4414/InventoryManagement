using System.Net;
using System.Net.Http.Json;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Integration.Tests.Suppliers;

public class SuppliersIntegrationTests : IntegrationTestBase
{
    public SuppliersIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateSupplier_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync();

        var createDto = new CreateSupplierDto
        {
            Name = "Importadora XYZ",
            Nit = "9876543210",
            Address = "Calle Comercio 456",
            Phone = "77654321",
            Email = "ventas@importadoraxyz.com",
            ContactName = "María González"
        };

        // Act
        var response = await PostAsync("/api/suppliers", createDto, user.Id);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdSupplier = await response.Content.ReadFromJsonAsync<Supplier>();
        Assert.NotNull(createdSupplier);
        Assert.Equal(createDto.Name, createdSupplier.Name);
        Assert.Equal(createDto.Nit, createdSupplier.Nit);
    }

    [Fact]
    public async Task GetAllSuppliers_ShouldReturnAllSuppliers()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        await CreateTestSupplierAsync(user.Id);

        // Act
        var response = await Client.GetAsync("/api/Suppliers");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var suppliers = await response.Content.ReadFromJsonAsync<List<Supplier>>();
        Assert.NotNull(suppliers);
        Assert.NotEmpty(suppliers);
    }

    [Fact]
    public async Task UpdateSupplier_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var supplier = await CreateTestSupplierAsync(user.Id);

        var updateDto = new UpdateSupplierDto
        {
            Name = "Proveedor ABC Actualizado",
            Nit = "1234567891",
            Address = "Nueva Dirección 789",
            Phone = "77999888",
            Email = "nuevo@proveedorabc.com",
            ContactName = "Roberto Silva"
        };

        // Act
        var response = await PutAsync($"/api/Suppliers/{supplier.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var getResponse = await Client.GetAsync($"/api/Suppliers/{supplier.Id}");
        var updatedSupplier = await getResponse.Content.ReadFromJsonAsync<Supplier>();

        Assert.NotNull(updatedSupplier);
        Assert.Equal(updateDto.Name, updatedSupplier.Name);
        Assert.Equal(updateDto.Email, updatedSupplier.Email);
    }

    [Fact]
    public async Task DeleteSupplier_AsAdmin_ShouldDeactivateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync("Admin");
        var supplier = await CreateTestSupplierAsync(user.Id);

        // Act
        var response = await DeleteAsync($"/api/Suppliers/{supplier.Id}", "Admin");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verificar que está inactivo
        var supplierInDb = await DbContext.Suppliers.FindAsync(supplier.Id);
        Assert.NotNull(supplierInDb);
        Assert.Equal(0, supplierInDb.Status);
    }
}