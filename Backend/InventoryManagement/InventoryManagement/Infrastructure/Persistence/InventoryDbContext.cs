using Microsoft.EntityFrameworkCore;
using InventoryManagement.Domain.Entities; 

namespace Infrastructure.Persistence;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
    }
    public DbSet<Product> Products { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Purchase> Purchases { get; set; }

    public DbSet<PurchaseDetail> PurchaseDetails { get; set; }
    public DbSet<SupplierProduct> SupplierProducts { get; set; }
    public DbSet<ProductPriceHistory> ProductPriceHistories { get; set; }

    
}