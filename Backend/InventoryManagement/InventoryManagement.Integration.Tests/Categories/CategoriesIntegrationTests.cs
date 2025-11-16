using System.Net;
using System.Net.Http.Json;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Integration.Tests.Categories;

public class CategoriesIntegrationTests : IntegrationTestBase
{
    public CategoriesIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateCategory_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var createDto = new CreateCategoryDto
        {
            Name = "Tecnología",
            Description = "Productos tecnológicos"
        };

        // Act
        var response = await PostAsync("/api/Categories", createDto, user.Id);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var createdCategory = await response.Content.ReadFromJsonAsync<Category>();
        Assert.NotNull(createdCategory);
        Assert.Equal(createDto.Name, createdCategory.Name);
        Assert.Equal(createDto.Description, createdCategory.Description);
    }

    [Fact]
    public async Task GetAllCategories_ShouldReturnAllCategories()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        await CreateTestCategoryAsync(user.Id);

        // Act
        var response = await Client.GetAsync("/api/Categories");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var categories = await response.Content.ReadFromJsonAsync<List<Category>>();
        Assert.NotNull(categories);
        Assert.NotEmpty(categories);
    }

    [Fact]
    public async Task UpdateCategory_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);

        var updateDto = new CreateCategoryDto
        {
            Name = "Electrónica Actualizada",
            Description = "Descripción actualizada"
        };

        // Act
        var response = await PutAsync($"/api/Categories/{category.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var getResponse = await Client.GetAsync($"/api/Categories/{category.Id}");
        var updatedCategory = await getResponse.Content.ReadFromJsonAsync<Category>();

        Assert.NotNull(updatedCategory);
        Assert.Equal(updateDto.Name, updatedCategory.Name);
        Assert.Equal(updateDto.Description, updatedCategory.Description);
    }

    [Fact]
    public async Task DeleteCategory_AsAdmin_ShouldDeactivateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync("Admin");
        var category = await CreateTestCategoryAsync(user.Id);

        // Act
        var response = await DeleteAsync($"/api/Categories/{category.Id}", "Admin");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verificar que está inactiva
        var categoryInDb = await DbContext.Categories.FindAsync(category.Id);
        Assert.NotNull(categoryInDb);
        Assert.Equal(0, categoryInDb.Status);
    }
}