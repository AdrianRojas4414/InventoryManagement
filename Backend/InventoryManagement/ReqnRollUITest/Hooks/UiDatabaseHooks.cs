using InventoryManagement.Infrastructure.Persistence;
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
            // 1. Load the configuration to get the connection string
            // We assume the .env or appsettings is in a similar location relative to the bin folder
            // Or you can hardcode the Staging connection string here for simplicity since it is a test project.
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            
            _config = builder.Build();

            // NOTE: Ensure this matches your Staging DB credentials
            // You might want to read this from your .env file logic like you did in the Factory
            var connectionString = "Server=localhost;Database=StagingInventoryManagementDB;User=root;Password=mysqlroosevelt14;"; 

            var optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            
            _dbContextOptions = optionsBuilder.Options;
        }

        [AfterScenario]
        public async Task CleanupDatabaseAfterScenario()
        {
            // 2. Create a temporary context just for cleaning
            using (var dbContext = new InventoryDbContext(_dbContextOptions))
            {
                await CleanupDatabaseAsync(dbContext);
            }
        }

        // 3. This is the exact logic copied from your IntegrationTestBase.cs
        private async Task CleanupDatabaseAsync(InventoryDbContext dbContext)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
        
            try
            {
                // Disable Foreign Keys to allow deleting in any order
                await dbContext.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0;");

                // Clear all tables
                dbContext.ProductPriceHistories.RemoveRange(dbContext.ProductPriceHistories);
                dbContext.PurchaseDetails.RemoveRange(dbContext.PurchaseDetails);
                dbContext.SupplierProducts.RemoveRange(dbContext.SupplierProducts);
                dbContext.Purchases.RemoveRange(dbContext.Purchases);
                dbContext.Products.RemoveRange(dbContext.Products);
                dbContext.Categories.RemoveRange(dbContext.Categories);
                dbContext.Suppliers.RemoveRange(dbContext.Suppliers);
                dbContext.Users.RemoveRange(dbContext.Users);
            
                await dbContext.SaveChangesAsync();

                // Re-enable Foreign Keys
                await dbContext.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1;");

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; 
            }
        }
    }
}