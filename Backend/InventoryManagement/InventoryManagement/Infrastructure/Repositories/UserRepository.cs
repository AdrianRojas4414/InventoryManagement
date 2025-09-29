using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

            return Ok(users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
                IsActive = u.IsActive
            }));
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = "TEMP_HASH", // TODO: encrypt password properly
                Role = dto.Role,
                IsActive = dto.IsActive,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow
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
            var success = await _userRepository.DeactivateAsync(id);

            if (!success)
                return NotFound(new { message = "User not found" });

            return Ok(new { message = "User deactivated (soft delete)" });
        }

        // PATCH: api/users/{id}/activate
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ActivateUser(ushort id)
        {
            var success = await _userRepository.ActivateAsync(id);

            if (!success)
                return NotFound(new { message = "User not found" });

            return Ok(new { message = "User reactivated" });
        }
    }
}
