using InventoryManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
}