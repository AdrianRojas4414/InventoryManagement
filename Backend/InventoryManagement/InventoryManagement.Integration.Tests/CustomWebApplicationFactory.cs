using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InventoryManagement.Infrastructure.Persistence;
using DotNetEnv;

namespace InventoryManagement.Integration.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remover el DbContext existente
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<InventoryDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Cargar variables de entorno
            Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", ".env"));

            var dbHost = "localhost";
            var dbName = "StagingInventoryManagementDB";
            var dbUser = Env.GetString("DB_USER");
            var dbPass = Env.GetString("DB_PASSWORD");

            var connectionString = $"Server={dbHost};Database={dbName};User={dbUser};Password={dbPass};";

            // Agregar el DbContext con la base de datos de staging
            services.AddDbContext<InventoryDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            // Construir el service provider
            var sp = services.BuildServiceProvider();

            // Crear el scope y obtener el contexto
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<InventoryDbContext>();

            // Asegurar que la base de datos esté creada
            db.Database.EnsureCreated();
        });
    }
}