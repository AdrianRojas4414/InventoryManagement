using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Usuario o contrase�a inv�lidos.");
            }

            // Buscar usuario por nombre de usuario
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);

            if (user == null)
            {
                return Unauthorized("Usuario no encontrado.");
            }

            // Validaci�n simple de contrase�a
            // En producci�n, aqu� deber�as comparar con un hash seguro
            if (user.PasswordHash != loginDto.Password)
            {
                return Unauthorized("Contrase�a incorrecta.");
            }

            Response.Cookies.Append("UserId", user.Id.ToString(), new CookieOptions { HttpOnly = true });
            Response.Cookies.Append("UserRole", user.Role, new CookieOptions { HttpOnly = true });

            // Preparar respuesta con datos del usuario
            var response = new LoginResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            };

            return Ok(response);
        }
    }
}
