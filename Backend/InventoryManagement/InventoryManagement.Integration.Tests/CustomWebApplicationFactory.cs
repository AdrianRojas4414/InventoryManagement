extern alias Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InventoryManagement.Infrastructure.Persistence;
using DotNetEnv;
using System.Linq; 

namespace InventoryManagement.Integration.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Web::Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", ".env"));

        
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptors = services.Where(
                d => d.ServiceType == typeof(InventoryDbContext) || 
                     d.ServiceType == typeof(DbContextOptions<InventoryDbContext>)
            ).ToList();

            foreach (var descriptor in dbContextDescriptors)
            {
                services.Remove(descriptor);
            }

            var dbHost = "localhost";
            var dbName = "StagingInventoryManagementDB";
            var dbUser = Env.GetString("DB_USER");
            var dbPass = Env.GetString("DB_PASSWORD");
            var connectionString = $"Server={dbHost};Database={dbName};User={dbUser};Password={dbPass};";

            services.AddDbContext<InventoryDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
            
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            
        });
    }
}