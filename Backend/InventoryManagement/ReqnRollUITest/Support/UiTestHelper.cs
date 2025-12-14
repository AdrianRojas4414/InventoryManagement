using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.ReqnrollUITest.Support
{
    public class UiTestHelper
    {
        private readonly InventoryDbContext _dbContext;

        public UiTestHelper(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> CreateTestUserAsync(string role = "Admin")
        {
            var user = new User
            {
                Username = role == "Admin" ? "AdminPedro" : "EmpleadoJuan",
                PasswordHash = "password123",
                FirstName = role == "Admin" ? "Pedro" : "Juan",
                LastName = role == "Admin" ? "García" : "López",
                Role = role,
                Status = 1,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            if (user.CreatedByUserId == null)
            {
                user.CreatedByUserId = user.Id;
                await _dbContext.SaveChangesAsync();
            }

            return user;
        }

        public async Task<User> EnsureUserExistsAsync(string role = "Admin")
        {
            var username = role == "Admin" ? "AdminPedro" : "EmpleadoJuan";
            
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Role == role);

            if (existingUser != null)
            {
                return existingUser;
            }

            return await CreateTestUserAsync(role);
        }

        public async Task<Category> CreateTestCategoryAsync(short userId, string? name = null, string? description = null)
        {
            var category = new Category
            {
                Name = name ?? "Categoría Temporal Test",
                Description = description ?? "Categoría para pruebas automáticas",
                Status = 1,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                CreatedByUserId = userId
            };

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<Category> EnsureCategoryExistsAsync(short userId)
        {
            var existingCategory = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Status == 1);

            if (existingCategory != null)
            {
                return existingCategory;
            }

            return await CreateTestCategoryAsync(userId);
        }

        public async Task<Category?> GetFirstActiveCategoryAsync()
        {
            return await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Status == 1);
        }

        public async Task<Domain.Entities.Supplier> CreateTestSupplierAsync(short userId, string? name = null, string? nit = null)
        {
            var supplier = new Domain.Entities.Supplier
            {
                Name = name ?? "ProveedorTest",
                Nit = nit ?? "12355487236",
                Phone = "70123456",
                Email = "test@gmail.com",
                ContactName = "Juan Perez",
                Address = "Av. Test 123",
                Status = 1,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                CreatedByUserId = userId
            };

            _dbContext.Suppliers.Add(supplier);
            await _dbContext.SaveChangesAsync();
            return supplier;
        }

        public async Task<Domain.Entities.Supplier> EnsureSupplierExistsAsync(short userId)
        {
            var existingSupplier = await _dbContext.Suppliers
                .FirstOrDefaultAsync(s => s.Status == 1);

            if (existingSupplier != null)
            {
                return existingSupplier;
            }

            return await CreateTestSupplierAsync(userId);
        }

        public async Task<Domain.Entities.Product> CreateTestProductAsync(short userId, short categoryId, string name, decimal price = 100)
        {
            var product = new Domain.Entities.Product
            {
                Name = name,
                Description = $"Descripción de {name}",
                SerialCode = 12547,
                CategoryId = categoryId,
                TotalStock = 10,
                Status = 1,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                CreatedByUserId = userId
            };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<Domain.Entities.Product> EnsureProductExistsAsync(string productName, short userId)
        {
            var category = await EnsureCategoryExistsAsync(userId);

            var existingProduct = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.Name == productName && p.Status == 1);

            if (existingProduct != null)
            {
                return existingProduct;
            }

            return await CreateTestProductAsync(userId, category.Id, productName);
        }
    }
}