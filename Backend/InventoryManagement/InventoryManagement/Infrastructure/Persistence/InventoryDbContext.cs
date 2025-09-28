using Microsoft.EntityFrameworkCore;
using InventoryManagement.Domain.Entities; 

namespace InventoryManagement.Infrastructure.Persistence;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PurchaseDetail>()
            .HasKey(pd => new { pd.PurchaseId, pd.ProductId });

        modelBuilder.Entity<SupplierProduct>()
            .HasKey(sp => new { sp.SupplierId, sp.ProductId });
        
        modelBuilder.Entity<ProductPriceHistory>()
            .HasKey(ph => new { ph.ProductId, ph.SupplierId, ph.PurchaseId });
    }
}