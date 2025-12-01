extern alias Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InventoryManagement.Infrastructure.Persistence;
using DotNetEnv;
using System.Linq; 
using MySqlConnector; // Asegúrate de tener este using para usar MySqlConnection directamente

namespace ReqnrollIntegrationTests.Fixtures;

public class CustomWebApplicationFactory : WebApplicationFactory<Web::Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        // Ajusta la ruta según la estructura de carpetas real
        Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", ".env"));
        
        builder.ConfigureServices(services =>
        {
            // 1. Remover la configuración existente del DbContext
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

            // 2. Crear dos cadenas de conexión:
            // - Una SIN base de datos (para tareas administrativas como crear/borrar la BD)
            var rootConnectionString = $"Server={dbHost};User={dbUser};Password={dbPass};";
            // - La completa para la aplicación
            var connectionString = $"Server={dbHost};Database={dbName};User={dbUser};Password={dbPass};";

            services.AddDbContext<InventoryDbContext>(options =>
            {
                // Usamos 'rootConnectionString' para el AutoDetect para evitar el error "Unknown database"
                options.UseMySql(connectionString, ServerVersion.AutoDetect(rootConnectionString));
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
            
            // 3. Gestión manual de la Base de Datos usando ADO.NET puro
            // Esto evita que EF Core falle al intentar conectarse a una BD que no existe
            using (var connection = new MySqlConnection(rootConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    // Borramos la BD si existe y la creamos de nuevo en limpio
                    command.CommandText = $"DROP DATABASE IF EXISTS `{dbName}`; CREATE DATABASE `{dbName}`;";
                    command.ExecuteNonQuery();
                }
            }

            // 4. Ahora que la BD existe, usamos EF Core para crear las tablas
            // Ya no necesitamos EnsureDeleted() ni EnsureCreated() para la BD, solo para el esquema
            // Pero EnsureCreated() es seguro llamarlo ahora porque la BD física ya existe.
            db.Database.EnsureCreated();
        });
    }
}