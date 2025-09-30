using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Persistence;
using InventoryManagement.Application.DTOs;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto, [FromHeader] string userRole)
    {
        if (userRole != "Admin")
        {
            return Forbid("Solo administradores pueden crear usuarios.");
        }

        if (userDto == null)
        {
            return BadRequest();
        }

        var newUser = new User
        {
            Username = userDto.Username,
            // falta hashear la contraseña
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
    // Dar de baja usuario (solo Admin)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeactivateUser(short id, [FromHeader] string userRole)
    {
        if (userRole != "Admin")
            return Forbid("Solo administradores pueden dar de baja usuarios.");

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound("Usuario no encontrado.");

        user.Status = 0;
        user.ModificationDate = DateTime.UtcNow;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return Ok("Usuario dado de baja correctamente.");
    }

    // Actualizar usuario (solo Admin)
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(short id, [FromBody] CreateUserDto userDto, [FromHeader] string userRole)
    {
        if (userRole != "Admin")
            return Forbid("Solo administradores pueden actualizar usuarios.");

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound("Usuario no encontrado.");

        user.Username = userDto.Username;
        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;
        user.SecondLastName = userDto.SecondLastName;
        user.Role = userDto.Role;
        user.ModificationDate = DateTime.UtcNow;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return Ok(user);
    }

    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateUserRole(short id, [FromBody] string newRole)
    {
        var userRole = Request.Cookies["UserRole"];
        if (userRole != "Admin") return Forbid("Solo administradores pueden cambiar roles.");

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound("Usuario no encontrado.");

        user.Role = newRole;
        user.ModificationDate = DateTime.UtcNow;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return Ok(user);
    }

    // Listar todos los usuarios (Admin y empleados)
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }
}