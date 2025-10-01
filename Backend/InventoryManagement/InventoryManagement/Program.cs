using Microsoft.EntityFrameworkCore;
using InventoryManagement.Infrastructure.Persistence;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Infrastructure.Repositories;
using DotNetEnv; 
using System.IO;
// using MySql.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"));

// Construye la cadena de conexión
var dbHost = "localhost";
var dbName = "inventorymanagementdb";
var dbUser = Env.GetString("DB_USER"); 
var dbPass = Env.GetString("DB_PASSWORD"); 

var connectionString = $"Server={dbHost};Database={dbName};User={dbUser};Password={dbPass};";

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// Add services to the container.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
