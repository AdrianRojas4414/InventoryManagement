using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using InventoryManagement.Infrastructure.Persistence;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Integration.Tests;

public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
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

    public async Task InitializeAsync()
    {
        // Limpieza de datos antes de cada test
        await CleanupDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        // Opcional: limpieza después del test
        return Task.CompletedTask;
    }

    //
    // --- AQUÍ ESTÁ LA CORRECCIÓN ---
    //
    private async Task CleanupDatabaseAsync()
    {
        // 1. Iniciar una transacción explícita
        using var transaction = await DbContext.Database.BeginTransactionAsync();
        
        try
        {
            // 2. Desactivar FK checks (DENTRO de la transacción)
            await DbContext.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0;");

            // 3. Eliminar datos
            DbContext.ProductPriceHistories.RemoveRange(DbContext.ProductPriceHistories);
            DbContext.PurchaseDetails.RemoveRange(DbContext.PurchaseDetails);
            DbContext.SupplierProducts.RemoveRange(DbContext.SupplierProducts);
            DbContext.Purchases.RemoveRange(DbContext.Purchases);
            DbContext.Products.RemoveRange(DbContext.Products);
            DbContext.Categories.RemoveRange(DbContext.Categories);
            DbContext.Suppliers.RemoveRange(DbContext.Suppliers);
            DbContext.Users.RemoveRange(DbContext.Users);
            
            // 4. Guardar todos los borrados (DENTRO de la transacción)
            await DbContext.SaveChangesAsync();

            // 5. Reactivar FK checks (DENTRO de la transacción)
            await DbContext.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1;");

            // 6. Si todo fue bien, comitear la transacción
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            // 7. Si algo falla, revertir todo
            await transaction.RollbackAsync();
            throw; // Relanzar la excepción para que el test falle y nos enteremos
        }
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
            Username = role == "Admin" ? "AdminPedro" : "EmpleadoJuan",
            PasswordHash = "password123",
            FirstName = role == "Admin" ? "Pedro" : "Juan",
            LastName = role == "Admin" ? "García" : "López",
            Role = role,
            Status = 1,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow
        };

        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();

        if (user.CreatedByUserId == null)
        {
            user.CreatedByUserId = user.Id;
            await DbContext.SaveChangesAsync();
        }
        return user;
    }

    protected async Task<Category> CreateTestCategoryAsync(short userId)
    {
        var category = new Category
        {
            Name = "Electrónica",
            Description = "Productos electrónicos",
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
            Name = "Proveedor ABC",
            Nit = "1234567890",
            Address = "Av. Siempre Viva 123",
            Phone = "77123456",
            Email = "contacto@gmail.com",
            ContactName = "Carlos Pérez",
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
        var product = new Product
        {
            SerialCode = 1001,
            Name = "Laptop Dell",
            Description = "Laptop empresarial",
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