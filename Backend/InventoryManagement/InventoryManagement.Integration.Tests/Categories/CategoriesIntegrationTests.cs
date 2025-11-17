extern alias Web; // Alias para tu proyecto Web

using System.Net;
using System.Net.Http.Json;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Integration.Tests;

public class CategoriesIntegrationTests : IntegrationTestBase
{
    public CategoriesIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    // -----------------------------------------------------------------
    // PRUEBA 1: CREATE (Insertar)
    // -----------------------------------------------------------------
    [Theory]
    // 1. Happy Path (Todos los datos correctos)
    [InlineData(true, "Tecnología", "Productos de tecnología de alta gama")]
    
    // 2. Unhappy Path (Datos que violan las validaciones del DTO)
    [InlineData(false, "A", // Nombre muy corto (falla MinimumLength = 3)
     // Descripción muy larga (falla MaxLength = 500)
     "Esta es una descripción extremadamente larga que está diseñada para fallar la validación de 500 caracteres. Repetiremos esta frase varias veces para asegurarnos de que el límite se exceda con creces. Esta es una descripción extremadamente larga que está diseñada para fallar la validación de 500 caracteres. Repetiremos esta frase varias veces para asegurarnos de que el límite se exceda con creces. Esta es una descripción extremadamente larga que está diseñada para fallar la validación de 500 caracteres. Repetiremos esta frase varias veces para asegurarnos de que el límite se exceda con creces.")]
    public async Task CreateCategory_Test(bool isHappyPath, string name, string description)
    {
        // Arrange
        var adminUser = await CreateTestUserAsync("Admin");
        var newCategoryDto = new CreateCategoryDto
        {
            Name = name,
            Description = description
        };

        // Act
        var response = await PostAsync("/api/categories", newCategoryDto, adminUser.Id);

        // Assert
        if (isHappyPath)
        {
            // Verificación del Happy Path
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var categoryFromResponse = await response.Content.ReadFromJsonAsync<Category>();
            Assert.NotNull(categoryFromResponse);

            // Verificamos la persistencia en la DB
            var categoryFromDb = await DbContext.Categories.FindAsync(categoryFromResponse.Id);
            Assert.NotNull(categoryFromDb);
            Assert.Equal(name, categoryFromDb.Name);
        }
        else
        {
            // Verificación del Unhappy Path (Error de validación del DTO)
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
    public async Task GetCategoryById_Test(bool isHappyPath)
    {
        // Arrange
        var adminUser = await CreateTestUserAsync("Admin");
        short categoryId;

        if (isHappyPath)
        {
            // 1. Creamos la categoría para poder encontrarla
            var category = await CreateTestCategoryAsync(adminUser.Id);
            categoryId = category.Id;
        }
        else
        {
            // 2. Usamos un ID que no existe
            categoryId = 999;
        }

        // Act
        var response = await Client.GetAsync($"/api/categories/{categoryId}");

        // Assert
        if (isHappyPath)
        {
            // Verificación del Happy Path
            response.EnsureSuccessStatusCode();
            var foundCategory = await response.Content.ReadFromJsonAsync<Category>();
            Assert.NotNull(foundCategory);
            Assert.Equal(categoryId, foundCategory.Id);
        }
        else
        {
            // Verificación del Unhappy Path (No encontrado)
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    // -----------------------------------------------------------------
    // PRUEBA 2.5: GET ALL (Select All)
    // -----------------------------------------------------------------
    [Theory]
    // 1. Happy Path (Hay categorías en la DB)
    [InlineData(true)]
    // 2. Unhappy Path (No hay categorías en la DB)
    [InlineData(false)]
    public async Task GetAllCategories_Test(bool isHappyPath)
    {
        // Arrange
        var adminUser = await CreateTestUserAsync("Admin");
        
        if (isHappyPath)
        {
            // Creamos varias categorías para el Happy Path
            await CreateTestCategoryAsync(adminUser.Id);
            await CreateTestCategoryAsync(adminUser.Id);
            await CreateTestCategoryAsync(adminUser.Id);
        }
        // Para Unhappy Path, no creamos nada (DB limpia)

        // Act
        var response = await Client.GetAsync("/api/categories");

        // Assert
        if (isHappyPath)
        {
            response.EnsureSuccessStatusCode();
            var categories = await response.Content.ReadFromJsonAsync<List<Category>>();
            Assert.NotNull(categories);
            Assert.True(categories.Count >= 3); // Al menos las 3 que creamos
        }
        else
        {
            response.EnsureSuccessStatusCode();
            var categories = await response.Content.ReadFromJsonAsync<List<Category>>();
            Assert.NotNull(categories);
            Assert.Empty(categories); // No debe haber categorías
        }
    }

    // -----------------------------------------------------------------
    // PRUEBA 3: UPDATE (Actualizar)
    // -----------------------------------------------------------------
    [Theory]
    // 1. Happy Path (Datos de actualización válidos)
    [InlineData(true, "Hogar (Actualizado)", "Nueva descripción")]
    
    // 2. Unhappy Path (Datos de actualización inválidos)
    [InlineData(false, "B", // Nombre muy corto
     "Esta es una descripción extremadamente larga que está diseñada para fallar la validación de 500 caracteres. Repetiremos esta frase varias veces para asegurarnos de que el límite se exceda con creces. Esta es una descripción extremadamente larga que está diseñada para fallar la validación de 500 caracteres. Repetiremos esta frase varias veces para asegurarnos de que el límite se exceda con creces. Esta es una descripción extremadamente larga que está diseñada para fallar la validación de 500 caracteres. Repetiremos esta frase varias veces para asegurarnos de que el límite se exceda con creces.")]
    public async Task UpdateCategory_Test(bool isHappyPath, string updatedName, string updatedDescription)
    {
        // Arrange
        var adminUser = await CreateTestUserAsync("Admin");
        var category = await CreateTestCategoryAsync(adminUser.Id); // Crea "Electrónica"
        
        var updateDto = new CreateCategoryDto
        {
            Name = updatedName,
            Description = updatedDescription
        };

        // Act
        var response = await PutAsync($"/api/categories/{category.Id}", updateDto);

        // Assert
        if (isHappyPath)
        {
            response.EnsureSuccessStatusCode();
            var categoryFromDb = await DbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == category.Id);
            
            Assert.NotNull(categoryFromDb);
            Assert.Equal(updatedName, categoryFromDb.Name);
        }
        else
        {
            // Verificación del Unhappy Path (Error de validación del DTO)
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // Verificamos que la DB NO se modificó
            var categoryFromDb = await DbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == category.Id);

            Assert.NotNull(categoryFromDb);
            Assert.Equal("Electrónica", categoryFromDb.Name); // Sigue teniendo el nombre original
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
    public async Task DeleteCategory_Test(bool isHappyPath, string userRole)
    {
        // Arrange
        var adminUser = await CreateTestUserAsync("Admin");
        var category = await CreateTestCategoryAsync(adminUser.Id);
        Assert.Equal(1, category.Status); // Verificamos que está activa

        // Act
        var deleteResponse = await DeleteAsync($"/api/categories/{category.Id}", userRole);

        // Assert
        if (isHappyPath)
        {
            deleteResponse.EnsureSuccessStatusCode();
            
            // Verificamos que se hizo el borrado lógico (Status = 0)
            await DbContext.Entry(category).ReloadAsync();
            Assert.Equal(0, category.Status);

            // Verificamos que ya no se puede obtener (da BadRequest "inactiva")
            var getResponse = await Client.GetAsync($"/api/categories/{category.Id}");
            Assert.Equal(HttpStatusCode.BadRequest, getResponse.StatusCode);
        }
        else
        {
            // Verificación del Unhappy Path (Permisos)
            Assert.Equal(HttpStatusCode.BadRequest, deleteResponse.StatusCode);
            var error = await deleteResponse.Content.ReadAsStringAsync();
            Assert.Contains("Solo los administradores pueden eliminar", error);

            // Verificamos que NO se borró (sigue activa)
            await DbContext.Entry(category).ReloadAsync();
            Assert.Equal(1, category.Status);
        }
    }
}