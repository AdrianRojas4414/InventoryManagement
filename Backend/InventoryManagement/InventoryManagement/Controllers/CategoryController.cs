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

    public CategoriesController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    // POST: api/categories -> Crear nueva categoría (solo Admin)
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
}