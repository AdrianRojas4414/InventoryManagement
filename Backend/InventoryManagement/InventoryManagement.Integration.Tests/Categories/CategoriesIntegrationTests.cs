using System.Net;
using System.Net.Http.Json;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Integration.Tests.Categories;

public class CategoriesIntegrationTests : IntegrationTestBase
{
    public CategoriesIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    #region CREATE Tests

    [Fact]
    public async Task CreateCategory_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync();

        var createDto = new CreateCategoryDto
        {
            Name = "dsdsdsdsd",
            Description = "New category description"
        };

        // Act
        var response = await PostAsync("/api/Categories", createDto, user.Id);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var createdCategory = await response.Content.ReadFromJsonAsync<Category>();
        Assert.NotNull(createdCategory);
        Assert.Equal(createDto.Name, createdCategory.Name);
        Assert.Equal(createDto.Description, createdCategory.Description);

        // Verificar que se puede obtener por ID
        var getResponse = await Client.GetAsync($"/api/Categories/{createdCategory.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var retrievedCategory = await getResponse.Content.ReadFromJsonAsync<Category>();
        Assert.NotNull(retrievedCategory);
        Assert.Equal(createdCategory.Id, retrievedCategory.Id);
        Assert.Equal(createDto.Name, retrievedCategory.Name);
    }

    [Fact]
    public async Task CreateCategory_WithMinimumValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync();

        var createDto = new CreateCategoryDto
        {
            Name = "ABC", // Nombre m�nimo de 3 caracteres
            Description = null
        };

        // Act
        var response = await PostAsync("/api/Categories", createDto, user.Id);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var createdCategory = await response.Content.ReadFromJsonAsync<Category>();
        Assert.NotNull(createdCategory);
        Assert.Equal(createDto.Name, createdCategory.Name);
    }

    #endregion

    #region READ Tests

    [Fact]
    public async Task GetAllCategories_ShouldReturnAllCategories()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        await CreateTestCategoryAsync(user.Id);
        await CreateTestCategoryAsync(user.Id);

        // Act
        var response = await Client.GetAsync("/api/Categories");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var categories = await response.Content.ReadFromJsonAsync<List<Category>>();
        Assert.NotNull(categories);
        Assert.True(categories.Count >= 2);
    }

    [Fact]
    public async Task GetCategoryById_WithExistingId_ShouldReturnCategory()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);

        // Act
        var response = await Client.GetAsync($"/api/Categories/{category.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var retrievedCategory = await response.Content.ReadFromJsonAsync<Category>();
        Assert.NotNull(retrievedCategory);
        Assert.Equal(category.Id, retrievedCategory.Id);
        Assert.Equal(category.Name, retrievedCategory.Name);
    }

    [Fact]
    public async Task GetCategoryById_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var response = await Client.GetAsync("/api/Categories/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCategoryById_WithInactiveCategory_ShouldReturnBadRequest()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);
        category.Status = 0;
        DbContext.Categories.Update(category);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/Categories/{category.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("inactiva", errorMessage.ToLower());
    }

    #endregion

    #region UPDATE Tests

    [Fact]
    public async Task UpdateCategory_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);

        var updateDto = new CreateCategoryDto
        {
            Name = $"UpdatedCategory_{Guid.NewGuid():N}",
            Description = "Updated description"
        };

        // Act
        var response = await PutAsync($"/api/Categories/{category.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verificar que la categor�a se actualiz� correctamente
        var getResponse = await Client.GetAsync($"/api/Categories/{category.Id}");
        var updatedCategory = await getResponse.Content.ReadFromJsonAsync<Category>();

        Assert.NotNull(updatedCategory);
        Assert.Equal(updateDto.Name, updatedCategory.Name);
        Assert.Equal(updateDto.Description, updatedCategory.Description);
    }

    [Fact]
    public async Task UpdateCategory_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var updateDto = new CreateCategoryDto
        {
            Name = "Updated Category",
            Description = "Description"
        };

        // Act
        var response = await PutAsync("/api/Categories/99999", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_WithInactiveCategory_ShouldReturnBadRequest()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var category = await CreateTestCategoryAsync(user.Id);
        category.Status = 0;
        DbContext.Categories.Update(category);
        await DbContext.SaveChangesAsync();

        var updateDto = new CreateCategoryDto
        {
            Name = "Updated Category",
            Description = "Description"
        };

        // Act
        var response = await PutAsync($"/api/Categories/{category.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("inactiva", errorMessage.ToLower());
    }

    #endregion

    #region DELETE Tests

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

        // Verificar que la categor�a est� inactiva
        var getResponse = await Client.GetAsync($"/api/Categories/{category.Id}");
        Assert.Equal(HttpStatusCode.BadRequest, getResponse.StatusCode);

        // Verificar en la base de datos que Status = 0
        var categoryInDb = await DbContext.Categories.FindAsync(category.Id);
        Assert.NotNull(categoryInDb);
        Assert.Equal(0, categoryInDb.Status);
    }

    [Fact]
    public async Task DeleteCategory_AsNonAdmin_ShouldReturnBadRequest()
    {
        // Arrange
        var user = await CreateTestUserAsync("Employee");
        var category = await CreateTestCategoryAsync(user.Id);

        // Act
        var response = await DeleteAsync($"/api/Categories/{category.Id}", "Employee");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("administradores", errorMessage.ToLower());
    }

    [Fact]
    public async Task DeleteCategory_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var response = await DeleteAsync("/api/Categories/99999", "Admin");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region ACTIVATE Tests

    [Fact]
    public async Task ActivateCategory_AsAdmin_ShouldActivateSuccessfully()
    {
        // Arrange
        var user = await CreateTestUserAsync("Admin");
        var category = await CreateTestCategoryAsync(user.Id);
        category.Status = 0;
        DbContext.Categories.Update(category);
        await DbContext.SaveChangesAsync();

        // Act
        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/Categories/{category.Id}/activate");
        request.Headers.Add("userRole", "Admin");
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verificar que la categor�a est� activa usando GetById
        var getResponse = await Client.GetAsync($"/api/Categories/{category.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var activatedCategory = await getResponse.Content.ReadFromJsonAsync<Category>();
        Assert.NotNull(activatedCategory);
        Assert.Equal(1, activatedCategory.Status);
    }

    [Fact]
    public async Task ActivateCategory_AsNonAdmin_ShouldReturnBadRequest()
    {
        // Arrange
        var user = await CreateTestUserAsync("Employee");
        var category = await CreateTestCategoryAsync(user.Id);
        category.Status = 0;
        DbContext.Categories.Update(category);
        await DbContext.SaveChangesAsync();

        // Act
        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/Categories/{category.Id}/activate");
        request.Headers.Add("userRole", "Employee");
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ActivateCategory_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/Categories/99999/activate");
        request.Headers.Add("userRole", "Admin");
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion
}