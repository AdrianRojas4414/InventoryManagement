using Microsoft.EntityFrameworkCore;
using InventoryManagement.Infrastructure.Persistence;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Infrastructure.Repositories;
using DotNetEnv;
using System.IO;


var builder = WebApplication.CreateBuilder(args);

// Cargar variables de entorno
Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"));

// Construye la cadena de conexión
var dbHost = "localhost";
var dbName = builder.Environment.IsDevelopment() 
    ? "StagingInventoryManagementDB" // Para tests
    : "StagingInventoryManagementDB";
var dbUser = Env.GetString("DB_USER");
var dbPass = Env.GetString("DB_PASSWORD");

var connectionString = $"Server={dbHost};Database={dbName};User={dbUser};Password={dbPass};";

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// Repositorios
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
builder.Services.AddScoped<IPurchaseDetailRepository, PurchaseDetailRepository>();

// Controllers y Swagger
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirección HTTPS
app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }