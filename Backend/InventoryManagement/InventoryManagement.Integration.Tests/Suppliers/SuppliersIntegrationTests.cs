extern alias Web; // Alias para tu proyecto Web

using System.Net;
using System.Net.Http.Json;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Integration.Tests;

public class SuppliersIntegrationTests : IntegrationTestBase
{
    public SuppliersIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    // -----------------------------------------------------------------
    // PRUEBA 1: CREATE (Insertar)
    // -----------------------------------------------------------------
    [Theory]
    // 1. Happy Path (Datos válidos que cumplen las nuevas reglas)
    [InlineData(true, "Proveedor Válido", "1234567", "+591 7777777", "valido@test.com", "Juan Perez", "Avenida Siempre Viva 123")]
    
    // 2. Unhappy Path (Datos que violan las nuevas reglas del DTO)
    [InlineData(false, "123", // Falla minlength 4 y pattern "no solo números"
     "123", // Falla minlength 7
     "telefono muy largo que excede los 25 caracteres", // Falla maxlength 25
     "not-an-email", // Falla EmailAddress
     "1", // Falla minlength 2 y pattern "solo letras"
     "Av." // Falla minlength 4
     )]
    public async Task CreateSupplier_Test(bool isHappyPath, string name, string nit, string phone, string email, string contact, string address)
    {
        // Arrange
        var adminUser = await CreateTestUserAsync("Admin");
        var newSupplierDto = new CreateSupplierDto
        {
            Name = name,
            Nit = nit,
            Phone = phone,
            Email = email,
            ContactName = contact,
            Address = address
        };
        
        if (isHappyPath)
        {
            // Para el Happy Path, asegurarnos de que el NIT/Email sean únicos
            // para no fallar por un test anterior (aunque la DB se limpia, es buena práctica)
            newSupplierDto.Nit = $"1234567-{Guid.NewGuid().ToString().Substring(0, 4)}";
            newSupplierDto.Email = $"valido-{Guid.NewGuid().ToString().Substring(0, 4)}@test.com";
        }

        // Act
        var response = await PostAsync("/api/suppliers", newSupplierDto, adminUser.Id);

        // Assert
        if (isHappyPath)
        {
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var supplierFromResponse = await response.Content.ReadFromJsonAsync<Supplier>();
            Assert.NotNull(supplierFromResponse);

            var supplierFromDb = await DbContext.Suppliers.FindAsync(supplierFromResponse.Id);
            Assert.NotNull(supplierFromDb);
            Assert.Equal(name, supplierFromDb.Name);
        }
        else
        {
            // Assert: El DTO debe ser rechazado por las validaciones
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
    public async Task GetSupplierById_Test(bool isHappyPath)
    {
        // Arrange
        var adminUser = await CreateTestUserAsync("Admin");
        short supplierId;

        if (isHappyPath)
        {
            var supplier = await CreateTestSupplierAsync(adminUser.Id);
            supplierId = supplier.Id;
        }
        else
        {
            supplierId = 999;
        }

        // Act
        var response = await Client.GetAsync($"/api/suppliers/{supplierId}");

        // Assert
        if (isHappyPath)
        {
            response.EnsureSuccessStatusCode();
            var supplier = await response.Content.ReadFromJsonAsync<Supplier>();
            Assert.NotNull(supplier);
            Assert.Equal(supplierId, supplier.Id);
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
    [InlineData(true, "Nombre Actualizado", "9876543", "+591 7654321", "update@test.com", "Pedro V", "Calle Falsa 123")]

    // 2. Unhappy Path (Datos inválidos)
    [InlineData(false, "456", // Falla minlength 4 y pattern "no solo números"
     "456", // Falla minlength 7
     "un telefono demasiado largo para ser real +591 7654321", // Falla maxlength 25
     "un-email-invalido", // Falla EmailAddress
     "2", // Falla minlength 2 y pattern "solo letras"
     "Cal" // Falla minlength 4
     )]
    public async Task UpdateSupplier_Test(bool isHappyPath, string name, string nit, string phone, string email, string contact, string address)
    {
        // Arrange
        var adminUser = await CreateTestUserAsync("Admin");
        var supplier = await CreateTestSupplierAsync(adminUser.Id); // Proveedor original

        var updateDto = new UpdateSupplierDto
        {
            Name = name,
            Nit = nit,
            Phone = phone,
            Email = email,
            ContactName = contact,
            Address = address
        };
        
        if (isHappyPath)
        {
            // Para el Happy Path, asegurarnos de que el NIT/Email sean únicos
            updateDto.Nit = $"9876543-{Guid.NewGuid().ToString().Substring(0, 4)}";
            updateDto.Email = $"update-{Guid.NewGuid().ToString().Substring(0, 4)}@test.com";
        }
        
        // Act
        var response = await PutAsync($"/api/suppliers/{supplier.Id}", updateDto);

        // Assert
        if (isHappyPath)
        {
            response.EnsureSuccessStatusCode();
            var supplierFromDb = await DbContext.Suppliers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == supplier.Id);
            Assert.NotNull(supplierFromDb);
            Assert.Equal(name, supplierFromDb.Name);
            Assert.Equal(updateDto.Nit, supplierFromDb.Nit);
        }
        else
        {
            // Assert: El DTO debe ser rechazado
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // Verificamos que no se actualizó
            var supplierFromDb = await DbContext.Suppliers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == supplier.Id);
            Assert.NotNull(supplierFromDb);
            Assert.Equal("Proveedor ABC", supplierFromDb.Name); // El nombre original del helper
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
    public async Task DeleteSupplier_Test(bool isHappyPath, string userRole)
    {
        // Arrange
        var adminUser = await CreateTestUserAsync("Admin");
        var supplier = await CreateTestSupplierAsync(adminUser.Id);
        Assert.Equal(1, supplier.Status);

        // Act
        var deleteResponse = await DeleteAsync($"/api/suppliers/{supplier.Id}", userRole);

        // Assert
        if (isHappyPath)
        {
            deleteResponse.EnsureSuccessStatusCode();
            await DbContext.Entry(supplier).ReloadAsync();
            Assert.Equal(0, supplier.Status); // Borrado Lógico

            var getResponse = await Client.GetAsync($"/api/suppliers/{supplier.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode); // No se debe poder encontrar
        }
        else
        {
            Assert.Equal(HttpStatusCode.BadRequest, deleteResponse.StatusCode);
            var error = await deleteResponse.Content.ReadAsStringAsync();
            Assert.Contains("Solo los administradores pueden dar de baja", error);
            
            await DbContext.Entry(supplier).ReloadAsync();
            Assert.Equal(1, supplier.Status); // Verifica que no se borró
        }
    }
}