using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Controllers;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Persistence;

namespace InventoryManagement.Application.Tests.Users;

public class UsersControllerTests
{
    private readonly InventoryDbContext _context;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new InventoryDbContext(options);
        _controller = new UsersController(_context);
    }

    [Fact]
    public async Task CreateUser_WhenUserIsAdmin_ShouldCreateUserSuccessfully()
    {
        var userDto = new CreateUserDto
        {
            Username = "testuser",
            Password = "password",
            FirstName = "Test",
            LastName = "User",
            Role = "Employee"
        };

        var result = await _controller.CreateUser(userDto, "Admin");

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal("testuser", returnedUser.Username);

        var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
        Assert.NotNull(userInDb);
    }

    [Theory]
    [InlineData("Employee")]
    public async Task CreateUser_WhenUserIsNotAdmin_ShouldReturnForbid(string nonAdminRole)
    {
        var userDto = new CreateUserDto { Username = "test", FirstName = "a", LastName = "b", Password = "c", Role = "d" };

        var result = await _controller.CreateUser(userDto, nonAdminRole);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task DeactivateUser_WhenUserExistsAndRoleIsAdmin_ShouldDeactivateUser()
    {
        var existingUser = new User { Id = 1, Username = "to_delete", Status = 1, FirstName = "a", LastName = "b", PasswordHash = "c", Role = "d" };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var result = await _controller.DeactivateUser(1, "Admin");

        Assert.IsType<OkObjectResult>(result);

        var deactivatedUser = await _context.Users.FindAsync((short)1);
        Assert.NotNull(deactivatedUser);
        Assert.Equal(0, deactivatedUser.Status);
    }

    [Fact]
    public async Task DeactivateUser_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        var result = await _controller.DeactivateUser(99, "Admin");

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetAllUsers_WhenUsersExist_ShouldReturnAllUsers()
    {
        _context.Users.AddRange(
            new User { Id = 1, Username = "user1", FirstName = "test", LastName = "Sanchez", PasswordHash = "Password", Role = "Admin"},
            new User { Id = 2, Username = "user2", FirstName = "test", LastName = "Sanchez", PasswordHash = "Password", Role = "Employee"}
        );
        await _context.SaveChangesAsync();

        var result = await _controller.GetAllUsers();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var users = Assert.IsAssignableFrom<List<User>>(okResult.Value);
        Assert.Equal(2, users.Count); 
    }
    
    [Fact]
    public async Task DeactivateUser_WhenRoleIsNotAdmin_ShouldReturnForbid()
    {
        var existingUser = new User { Id = 1, Username = "test", FirstName = "test", LastName = "Sanchez", PasswordHash = "Password", Role = "Employee" };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var result = await _controller.DeactivateUser(1, "Employee");

        Assert.IsType<ForbidResult>(result); 
    }
}