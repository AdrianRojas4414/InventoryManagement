using InventoryManagement.Infrastructure.Persistence;
using InventoryManagement.ReqnrollUITest.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reqnroll;
using System;
using System.IO;
using System.Threading.Tasks;

namespace InventoryManagement.ReqnrollUITest.Hooks
{
    [Binding]
    public class UiDatabaseHooks
    {
        private static IConfigurationRoot _config;
        private static DbContextOptions<InventoryDbContext> _dbContextOptions;

        [BeforeTestRun]
        public static void ConfigureDatabaseAccess()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            
            _config = builder.Build();

            var connectionString = "Server=localhost;Database=StagingInventoryManagementDB;User=root;Password=mysqlroosevelt14;"; 

            var optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            
            _dbContextOptions = optionsBuilder.Options;
        }

        [AfterScenario]
        public async Task CleanupDatabaseAfterScenario()
        {
            using (var dbContext = new InventoryDbContext(_dbContextOptions))
            {
                await CleanupDatabaseAsync(dbContext);
                
                // Recreate essential data after cleanup
                await RecreateEssentialDataAsync(dbContext);
            }
        }

        private async Task CleanupDatabaseAsync(InventoryDbContext dbContext)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
        
            try
            {
                await dbContext.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0;");

                dbContext.ProductPriceHistories.RemoveRange(dbContext.ProductPriceHistories);
                dbContext.PurchaseDetails.RemoveRange(dbContext.PurchaseDetails);
                dbContext.SupplierProducts.RemoveRange(dbContext.SupplierProducts);
                dbContext.Purchases.RemoveRange(dbContext.Purchases);
                dbContext.Products.RemoveRange(dbContext.Products);
                dbContext.Categories.RemoveRange(dbContext.Categories);
                dbContext.Suppliers.RemoveRange(dbContext.Suppliers);
                dbContext.Users.RemoveRange(dbContext.Users);
            
                await dbContext.SaveChangesAsync();

                await dbContext.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1;");

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; 
            }
        }

        private async Task RecreateEssentialDataAsync(InventoryDbContext dbContext)
        {
            var helper = new UiTestHelper(dbContext);
            
            // Create Admin user
            var adminUser = await helper.CreateTestUserAsync("Admin");
            
            // Create a default entities
            await helper.CreateTestCategoryAsync(adminUser.Id, "Tecnología", "Productos tecnológicos");
            await helper.CreateTestSupplierAsync(adminUser.Id, "ProveedorABDCDS", "12548752023");
            await helper.EnsureProductExistsAsync("Laptop Dell", adminUser.Id);
            await helper.EnsureProductExistsAsync("Teclado Mecanico", adminUser.Id);
        }
    }
}