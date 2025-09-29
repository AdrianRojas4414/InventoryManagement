using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Ruta base: api/users
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users.Select(user => new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                IsActive = user.IsActive
            }));
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = "TEMP_HASH", // TODO: encriptar contraseña
                Role = dto.Role,
                IsActive = dto.IsActive
            };

            var created = await _userRepository.AddAsync(user);

            return Ok(new UserDto
            {
                Id = created.Id,
                Username = created.Username,
                Role = created.Role,
                IsActive = created.IsActive
            });
        }

        // PATCH: api/users/{id}/deactivate
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(ushort id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            user.IsActive = false;
            await _userRepository.UpdateAsync(user);

            return Ok(new { message = "User deactivated (soft delete)" });
        }

        // PATCH: api/users/{id}/activate
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ActivateUser(ushort id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            user.IsActive = true;
            await _userRepository.UpdateAsync(user);

            return Ok(new { message = "User reactivated" });
        }
    }
}
