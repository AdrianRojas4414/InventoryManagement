using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Persistence;
using InventoryManagement.Application.DTOs;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly InventoryDbContext _context;

    // El DbContext se inyecta gracias a la configuración en Program.cs
    public UsersController(InventoryDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
    {
        if (userDto == null)
        {
            return BadRequest();
        }

        var newUser = new User
        {
            Username = userDto.Username,
            //falta hashear la contrasena
            PasswordHash = userDto.Password,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            SecondLastName = userDto.SecondLastName,
            Role = userDto.Role,
            Status = 1,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
        };

        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        return Ok(newUser);
    }
}