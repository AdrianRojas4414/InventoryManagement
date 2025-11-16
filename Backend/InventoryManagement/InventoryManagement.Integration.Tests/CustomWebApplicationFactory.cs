extern alias Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InventoryManagement.Infrastructure.Persistence;
using DotNetEnv;

namespace InventoryManagement.Integration.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Web::Program>
{
     protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Cargar variables de entorno
        Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", ".env"));

        builder.ConfigureServices(services =>
        {
            // Remover el DbContext existente
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<InventoryDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Agregar DbContext usando la base de datos de pruebas
            var dbHost = "localhost";
            var dbName = "StagingInventoryManagementDB"; // Base de datos separada para tests
            var dbUser = Env.GetString("DB_USER");
            var dbPass = Env.GetString("DB_PASSWORD");

            var connectionString = $"Server={dbHost};Database={dbName};User={dbUser};Password={dbPass};";

            services.AddDbContext<InventoryDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            // Crear la base de datos y aplicar migraciones
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<InventoryDbContext>();
            
            db.Database.EnsureCreated();
        });
    }
}