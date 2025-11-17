extern alias Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InventoryManagement.Infrastructure.Persistence;
using DotNetEnv;
using System.Linq; // <-- Asegúrate de tener este using

namespace InventoryManagement.Integration.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Web::Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", ".env"));

        // Ya que Program.cs hardcodea la connection string,
        // DEBEMOS reemplazar el servicio DbContext.
        
        builder.ConfigureServices(services =>
        {
            // --- INICIO DE LA LÓGICA CORRECTA ---

            // 1. Remover el DbContext de PRODUCCIÓN que Program.cs registra.
            // Usamos un método robusto para buscar y eliminar todas las
            // registraciones relacionadas con InventoryDbContext.
            var dbContextDescriptors = services.Where(
                d => d.ServiceType == typeof(InventoryDbContext) || 
                     d.ServiceType == typeof(DbContextOptions<InventoryDbContext>)
            ).ToList();

            foreach (var descriptor in dbContextDescriptors)
            {
                services.Remove(descriptor);
            }

            // 2. Construir la connection string de STAGING
            var dbHost = "localhost";
            var dbName = "StagingInventoryManagementDB"; // <-- Tu DB de Staging
            var dbUser = Env.GetString("DB_USER");
            var dbPass = Env.GetString("DB_PASSWORD");
            var connectionString = $"Server={dbHost};Database={dbName};User={dbUser};Password={dbPass};";

            // 3. Registrar nuestro DbContext para que apunte a STAGING
            services.AddDbContext<InventoryDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            // 4. Construir un ServiceProvider TEMPORAL para poder
            //    limpiar la base de datos de STAGING antes de la prueba.
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
            
            // Borra y crea la DB para asegurar un estado limpio
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            
            // --- FIN DE LA LÓGICA CORRECTA ---
        });
    }
}