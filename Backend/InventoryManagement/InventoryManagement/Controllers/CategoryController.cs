using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private const string CategoryNotFoundMessage = "Categoría no encontrada.";

    public CategoriesController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    // POST: api/categories -> Crear nueva categor�a (solo Admin)
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto,[FromHeader] short userId)
    {
        var newCategory = new Category
        {
            Name = categoryDto.Name,
            Description = categoryDto.Description,
            Status = 1,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        await _categoryRepository.AddAsync(newCategory);

        return Ok(newCategory);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return Ok(categories);
    }

    // GET: api/categories/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(short id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound(CategoryNotFoundMessage);
        }
        if (category.Status == 0)
        {
            return BadRequest("La categoría está inactiva.");
        }
        return Ok(category);
    }

    // PUT: api/categories/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(short id, [FromBody] CreateCategoryDto categoryDto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound(CategoryNotFoundMessage);
        }
        if (category.Status == 0)
        {
            return BadRequest("La categoría está inactiva.");
        }

        category.Name = categoryDto.Name;
        category.Description = categoryDto.Description;

        await _categoryRepository.UpdateAsync(category);
        return Ok(category);
    }

    // DELETE: api/categories/{id} (Borrado lógico - solo Admin)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(short id, [FromHeader] string userRole)
    {
        if (userRole != "Admin")
        {
            return BadRequest("Acción no permitida. Solo los administradores pueden eliminar categorías.");
        }

        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound(CategoryNotFoundMessage);
        }

        await _categoryRepository.DeleteAsync(id);
        return Ok("Categoría eliminada correctamente.");
    }

    // PUT: api/categories/{id}/activate (solo Admin)
    [HttpPut("{id}/activate")]
    public async Task<IActionResult> ActivateCategory(short id, [FromHeader] string userRole)
    {
        if (userRole != "Admin")
        {
            return BadRequest("Solo los administradores pueden activar categorías.");
        }

        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound(CategoryNotFoundMessage);
        }

        category.Status = 1;
        category.ModificationDate = DateTime.UtcNow;

        await _categoryRepository.UpdateAsync(category);
        return Ok("Categoría activada correctamente.");
    }
}