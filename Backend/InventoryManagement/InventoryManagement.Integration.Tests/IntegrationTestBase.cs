using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using InventoryManagement.Infrastructure.Persistence;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Integration.Tests;

public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly InventoryDbContext DbContext;

    public IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();

        var scope = factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    }

    protected async Task<T?> GetAsync<T>(string url)
    {
        var response = await Client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    protected async Task<HttpResponseMessage> PostAsync<T>(string url, T content, short? userId = null, string? userRole = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(content)
        };

        if (userId.HasValue)
            request.Headers.Add("userId", userId.Value.ToString());

        if (userRole != null)
            request.Headers.Add("userRole", userRole);

        return await Client.SendAsync(request);
    }

    protected async Task<HttpResponseMessage> PutAsync<T>(string url, T content, short? userId = null, string? userRole = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = JsonContent.Create(content)
        };

        if (userId.HasValue)
            request.Headers.Add("userId", userId.Value.ToString());

        if (userRole != null)
            request.Headers.Add("userRole", userRole);

        return await Client.SendAsync(request);
    }

    protected async Task<HttpResponseMessage> DeleteAsync(string url, string? userRole = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, url);

        if (userRole != null)
            request.Headers.Add("userRole", userRole);

        return await Client.SendAsync(request);
    }

    protected async Task<User> CreateTestUserAsync(string role = "Admin")
    {
        var user = new User
        {
            Username = $"testuser_{Guid.NewGuid():N}",
            PasswordHash = "testpassword",
            FirstName = "Test",
            LastName = "User",
            Role = role,
            Status = 1,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow
        };

        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();
        return user;
    }

    protected async Task<Category> CreateTestCategoryAsync(short userId)
    {
        var category = new Category
        {
            Name = $"TestCategory_{Guid.NewGuid():N}",
            Description = "Test category description",
            Status = 1,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        DbContext.Categories.Add(category);
        await DbContext.SaveChangesAsync();
        return category;
    }

    protected async Task<Supplier> CreateTestSupplierAsync(short userId)
    {
        var supplier = new Supplier
        {
            Name = $"TestSupplier_{Guid.NewGuid():N}",
            Nit = $"{Random.Shared.Next(1000000, 9999999)}",
            Address = "Test Address",
            Phone = "12345678",
            Email = $"test_{Guid.NewGuid():N}@test.com",
            ContactName = "Test Contact",
            Status = 1,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        DbContext.Suppliers.Add(supplier);
        await DbContext.SaveChangesAsync();
        return supplier;
    }

    protected async Task<Product> CreateTestProductAsync(short categoryId, short userId)
    {
        var serialCode = (short)Random.Shared.Next(1000, 9999);

        var product = new Product
        {
            SerialCode = serialCode,
            Name = $"TestProduct_{Guid.NewGuid():N}",
            Description = "Test product description",
            CategoryId = categoryId,
            TotalStock = 10,
            Status = 1,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        DbContext.Products.Add(product);
        await DbContext.SaveChangesAsync();
        return product;
    }
}