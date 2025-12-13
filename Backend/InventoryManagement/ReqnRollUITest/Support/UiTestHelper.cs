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

        /// <summary>
        /// Creates a test user with specific credentials that match LoginPage expectations
        /// </summary>
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

        /// <summary>
        /// Ensures a user exists for the given role
        /// </summary>
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

        /// <summary>
        /// Creates a test category for product creation
        /// </summary>
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

        /// <summary>
        /// Ensures at least one category exists in the database
        /// Returns the first active category or creates one if none exist
        /// </summary>
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

        /// <summary>
        /// Gets the first active category (useful after ensuring one exists)
        /// </summary>
        public async Task<Category?> GetFirstActiveCategoryAsync()
        {
            return await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Status == 1);
        }
    }
}