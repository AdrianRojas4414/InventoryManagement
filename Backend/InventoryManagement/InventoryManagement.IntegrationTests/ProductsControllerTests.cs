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

public class ProductsControllerTests : TestBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        // Instanciamos todas las dependencias del controlador
        _productRepository = new ProductRepository(DbContext);
        _categoryRepository = new CategoryRepository(DbContext);
        _controller = new ProductsController(_productRepository, _categoryRepository);
    }

    // Cubre: INSERT (Válido e Inválido - FK y UNIQUE)
    [Theory]
    [InlineData(555, "Nuevo Producto", DefaultCategoryId, true)] // Válido
    [InlineData(666, "Sin Categoría", 999, false)] // Inválido (Categoría no existe)
    [InlineData(100, "Serial Duplicado", DefaultCategoryId, false)] // Inválido (SerialCode de TestBase)
    [InlineData(777, "Default Product", DefaultCategoryId, false)] // Inválido (Nombre de TestBase)
    public async Task CreateProduct_ShouldHandleValidAndInvalid(short serial, string name, short catId, bool isValid)
    {
        // Arrange
        var dto = new CreateProductDto { SerialCode = serial, Name = name, CategoryId = catId };

        // Act
        var actionResult = await _controller.CreateProduct(dto, AdminUserId);

        // Assert (API)
        if (isValid)
        {
            Assert.IsType<OkObjectResult>(actionResult);
        }
        else
        {
            // Tu controlador devuelve BadRequest por FK o UNIQUE
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            var errorMessage = badRequestResult.Value.ToString();
            Assert.NotNull(errorMessage);
        }

        // Assert (Integración y Persistencia - Happy Path)
        if (isValid)
        {
            var productInDb = await DbContext.Products.FirstOrDefaultAsync(p => p.Name == name);
            Assert.NotNull(productInDb);
            Assert.Equal(serial, productInDb.SerialCode);
            Assert.Equal(catId, productInDb.CategoryId);
        }
    }

    // Cubre: UPDATE (Válido e Inválido)
    [Theory]
    [InlineData(DefaultProductId, "Nombre Actualizado", DefaultCategoryId, true)] // Válido
    [InlineData(999, "Update Fantasma", DefaultCategoryId, false)] // Inválido (ID no existe)
    [InlineData(DefaultProductId, "Update Cat Inválida", 999, false)] // Inválido (Cat no existe)
    public async Task UpdateProduct_ShouldHandleValidAndInvalid(short id, string name, short catId, bool isValid)
    {
        // Arrange
        var dto = new CreateProductDto { Name = name, SerialCode = 100, CategoryId = catId };

        // Act
        var actionResult = await _controller.UpdateProduct(id, dto);

        // Assert (API)
        if (isValid)
            Assert.IsType<OkObjectResult>(actionResult);
        else if (id == 999)
            Assert.IsType<NotFoundObjectResult>(actionResult);
        else
            Assert.IsType<BadRequestObjectResult>(actionResult); // Cat 999
    }

    // Cubre: DELETE (Admin vs Otro Usuario)
    [Theory]
    [InlineData(AdminUserRole, typeof(OkObjectResult))]
    [InlineData(EmployeeUserRole, typeof(BadRequestObjectResult))]
    public async Task DeleteProduct_ShouldRespectRolePermissions(string role, Type expectedResult)
    {
        // Act
        var actionResult = await _controller.DeleteProduct(DefaultProductId, role);
        
        // Assert
        Assert.IsType(expectedResult, actionResult);
    }

    // Cubre: SELECT (Mostrar todos)
    [Fact]
    public async Task GetAllProducts_ShouldReturnAll()
    {
        // Act
        var actionResult = await _controller.GetAllProducts();
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var products = okResult.Value as List<Product>;
        Assert.NotNull(products);
        Assert.Contains(products, p => p.Name == "Default Product");
    }
}