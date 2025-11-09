// 1. Declarar el ÚNICO alias que usaremos
extern alias Web;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

// 2. Usar el alias "Web::" para encontrar las clases
//    Tu proyecto "Web" ya contiene las referencias a Domain e Infrastructure
using Web::InventoryManagement.Infrastructure.Persistence;
using Web::InventoryManagement.Domain.Entities;

namespace InventoryManagement.IntegrationTests;

public abstract class TestBase : IDisposable
{
    private readonly SqliteConnection _connection;
    protected readonly InventoryDbContext DbContext;

    // IDs (vienen de 'Web::InventoryManagement.Domain.Entities')
    protected const short AdminUserId = 1;
    protected const string AdminUserRole = "Admin";
    protected const short EmployeeUserId = 2;
    protected const string EmployeeUserRole = "Employee";
    protected const short DefaultCategoryId = 1;
    protected const short DefaultSupplierId = 1;
    protected const short DefaultProductId = 1;

    protected TestBase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseSqlite(_connection)
            .Options;

        // DbContext viene de "Web::..."
        DbContext = new InventoryDbContext(options);
        DbContext.Database.EnsureCreated();
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        // Entidades vienen de "Web::..."
        var users = new[]
        {
            new User { Id = AdminUserId, Username = "admin", PasswordHash = "...", FirstName = "Admin", LastName = "User", Role = AdminUserRole, Status = 1 },
            new User { Id = EmployeeUserId, Username = "employee", PasswordHash = "...", FirstName = "Employee", LastName = "User", Role = EmployeeUserRole, Status = 1 }
        };
        DbContext.Users.AddRange(users);

        var category = new Category { Id = DefaultCategoryId, Name = "Default Category", CreatedByUserId = AdminUserId, Status = 1 };
        DbContext.Categories.Add(category);

        var supplier = new Supplier { Id = DefaultSupplierId, Name = "Default Supplier", Nit = "123456", Email = "default@supplier.com", CreatedByUserId = AdminUserId, Status = 1 };
        DbContext.Suppliers.Add(supplier);

        var product = new Product { Id = DefaultProductId, Name = "Default Product", SerialCode = 100, CategoryId = DefaultCategoryId, TotalStock = 50, CreatedByUserId = AdminUserId, Status = 1 };
        DbContext.Products.Add(product);
        
        DbContext.SaveChanges();
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
        DbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}