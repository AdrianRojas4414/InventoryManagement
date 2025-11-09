extern alias Web;

using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

// 2. Usar "Web::" para TODOS los 'using' de tu proyecto
using Web::InventoryManagement.Application.DTOs;
using Web::InventoryManagement.Application.Interfaces;
using Web::InventoryManagement.Domain.Entities;
using Web::InventoryManagement.Infrastructure.Repositories;
using Web::InventoryManagement.Controllers;

namespace InventoryManagement.IntegrationTests;

public class SuppliersControllerTests : TestBase
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly SuppliersController _controller;

    public SuppliersControllerTests()
    {
        // Instanciamos todo desde el "Mundo Web"
        _supplierRepository = new SupplierRepository(DbContext);
        _controller = new SuppliersController(_supplierRepository);
    }

    // Cubre: INSERT (Válido e Inválido - UNIQUE)
    [Theory]
    [InlineData("Proveedor Válido", "777888", "valido@test.com", true)] // Válido
    [InlineData("NIT Duplicado", "123456", "test1@test.com", false)] // Inválido (NIT de TestBase)
    [InlineData("Email Duplicado", "999999", "default@supplier.com", false)] // Inválido (Email de TestBase)
    public async Task CreateSupplier_ShouldHandleValidAndInvalid(string name, string nit, string email, bool isValid)
    {
        // Arrange
        var dto = new CreateSupplierDto { Name = name, Nit = nit, Email = email };

        // Act
        var actionResult = await _controller.CreateSupplier(dto, AdminUserId);

        // Assert (API)
        if (isValid)
        {
            Assert.IsType<CreatedAtActionResult>(actionResult);
        }
        else
        {
            // Tu controlador captura la DbUpdateException y devuelve BadRequest
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            var errorMessage = badRequestResult.Value.ToString();
            Assert.Contains("Ya existe", errorMessage); // Verifica el mensaje de error
        }

        // Assert (Integración y Persistencia - Happy Path)
        if (isValid)
        {
            var supplierInDb = await DbContext.Suppliers.FirstOrDefaultAsync(s => s.Name == name);
            Assert.NotNull(supplierInDb);
            Assert.Equal(nit, supplierInDb.Nit);
        }
    }

    // Cubre: UPDATE (Válido e Inválido)
    [Theory]
    [InlineData(DefaultSupplierId, "Nombre Actualizado", true)] // Válido
    [InlineData(999, "Update Fantasma", false)] // Inválido (ID no existe)
    public async Task UpdateSupplier_ShouldHandleValidAndInvalid(short id, string name, bool isValid)
    {
        // Arrange
        // Usamos el NIT/Email existentes para evitar fallos de UNIQUE en la actualización
        var dto = new UpdateSupplierDto { Name = name, Nit = "123456", Email = "default@supplier.com" };

        // Act
        var actionResult = await _controller.UpdateSupplier(id, dto);

        // Assert (API)
        if (isValid)
            Assert.IsType<OkObjectResult>(actionResult);
        else
            Assert.IsType<NotFoundObjectResult>(actionResult);

        // Assert (Integración y Persistencia - Happy Path)
        if (isValid)
        {
            var supplierInDb = await DbContext.Suppliers.FindAsync(id);
            Assert.Equal(name, supplierInDb!.Name);
        }
    }

    // Cubre: DELETE (Admin vs Otro Usuario)
    [Theory]
    [InlineData(AdminUserRole, typeof(OkObjectResult))] // Admin PUEDE
    [InlineData(EmployeeUserRole, typeof(BadRequestObjectResult))] // Empleado NO PUEDE
    public async Task DeactivateSupplier_ShouldRespectRolePermissions(string role, Type expectedResult)
    {
        // Act
        var actionResult = await _controller.DeactivateSupplier(DefaultSupplierId, role);

        // Assert
        Assert.IsType(expectedResult, actionResult);

        // Assert (Integración y Persistencia - Happy Path)
        if (expectedResult == typeof(OkObjectResult))
        {
            // Verificamos que el borrado lógico funcionó
            var supplierInDb = await DbContext.Suppliers.FindAsync(DefaultSupplierId);
            Assert.Equal((byte)0, supplierInDb!.Status); // 0 = Inactivo
        }
    }

    // Cubre: SELECT (Mostrar todos)
    [Fact]
    public async Task GetAllSuppliers_ShouldReturnAll()
    {
        // Act
        var actionResult = await _controller.GetAllSuppliers();
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var suppliers = okResult.Value as List<Supplier>;
        Assert.NotNull(suppliers);
        Assert.Contains(suppliers, s => s.Name == "Default Supplier");
    }
}