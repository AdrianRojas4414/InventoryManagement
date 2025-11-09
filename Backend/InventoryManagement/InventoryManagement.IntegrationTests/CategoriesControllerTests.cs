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

public class CategoriesControllerTests : TestBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly CategoriesController _controller;

    public CategoriesControllerTests()
    {
        // 3. Instanciamos todo desde el "Mundo Web"
        //    ¡Ya no hay errores de conversión!
        _categoryRepository = new CategoryRepository(DbContext);
        _controller = new CategoriesController(_categoryRepository);
    }

    // Cubre: INSERT (Válido e Inválido)
    [Theory]
    [InlineData("Lácteos", "Productos lácteos", true)] // Válido
    [InlineData("a", "Desc", false)] // Inválido (Nombre corto por DTO)
    public async Task CreateCategory_ShouldHandleValidAndInvalid(string name, string desc, bool isValid)
    {
        // Arrange
        // 4. El DTO viene de "Web::..."
        var dto = new CreateCategoryDto { Name = name, Description = desc };

        // Act
        // 5. El Controller (de "Web::") acepta el DTO (de "Web::")
        var actionResult = await _controller.CreateCategory(dto, AdminUserId);

        // Assert (API)
        if (isValid)
        {
            Assert.IsType<OkObjectResult>(actionResult);
        }
        else
        {
            // Tu controlador devuelve BadRequest si el DTO es inválido
            // (lo cual debería fallar la validación [StringLength(..., MinimumLength = 3)])
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        // Assert (Integración y Persistencia - Happy Path)
        if (isValid)
        {
            var categoryInDb = await DbContext.Categories.FirstOrDefaultAsync(c => c.Name == name);
            Assert.NotNull(categoryInDb);
            Assert.Equal("Lácteos", categoryInDb.Name);
        }
    }

    // Cubre: UPDATE (Válido e Inválido)
    [Theory]
    [InlineData(DefaultCategoryId, "Electrónica", true)] // Válido
    [InlineData(999, "Update Fantasma", false)] // Inválido (ID no existe)
    public async Task UpdateCategory_ShouldHandleValidAndInvalid(short id, string name, bool isValid)
    {
        // Arrange
        var dto = new CreateCategoryDto { Name = name, Description = "Test" };

        // Act
        var actionResult = await _controller.UpdateCategory(id, dto);

        // Assert (API)
        if (isValid)
            Assert.IsType<OkObjectResult>(actionResult);
        else
            Assert.IsType<NotFoundObjectResult>(actionResult); // Tu controller devuelve NotFound

        // Assert (Integración y Persistencia - Happy Path)
        if (isValid)
        {
            var categoryInDb = await DbContext.Categories.FindAsync(id);
            Assert.Equal(name, categoryInDb!.Name);
        }
    }

    // Cubre: DELETE (Admin vs Otro Usuario)
    [Theory]
    [InlineData(AdminUserRole, typeof(OkObjectResult))] // Admin PUEDE
    [InlineData(EmployeeUserRole, typeof(BadRequestObjectResult))] // Empleado NO PUEDE
    public async Task DeleteCategory_ShouldRespectRolePermissions(string role, Type expectedResult)
    {
        // Arrange
        var categoryId = DefaultCategoryId;

        // Act
        var actionResult = await _controller.DeleteCategory(categoryId, role);

        // Assert
        Assert.IsType(expectedResult, actionResult);
    }

    // Cubre: SELECT (Mostrar todos)
    [Fact]
    public async Task GetAllCategories_ShouldReturnAll()
    {
        // Act
        var actionResult = await _controller.GetAllCategories();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var categories = okResult.Value as List<Category>;
        
        Assert.NotNull(categories);
        Assert.Contains(categories, c => c.Name == "Default Category");
    }
}