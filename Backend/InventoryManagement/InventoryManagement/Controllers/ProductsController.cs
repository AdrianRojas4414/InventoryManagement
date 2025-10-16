using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private const string ProductNotFoundMessage = "Producto no encontrado.";

    public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    // GET: api/products -> Criterio: Visualizar todos los productos
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productRepository.GetAllAsync();
        return Ok(products);
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(short id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null || product.Status == 0)
        {
            return NotFound(ProductNotFoundMessage);
        }
        return Ok(product);
    }

    // POST: api/products -> Criterio: Registrar nuevos productos con c�digo de barras
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto, [FromHeader] short userId)
    {

        // Validar que la categoría exista
        var category_val = await _categoryRepository.GetByIdAsync(productDto.CategoryId);
        if (category_val == null)
        {
            return BadRequest("La categoría seleccionada no existe.");
        }
        if (category_val.Status == 0)
        {
            return BadRequest("La categoría seleccionada está inactiva.");
        }

        var newProduct = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            CategoryId = productDto.CategoryId,
            TotalStock = productDto.TotalStock,
            Status = 1,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
            CreatedByUserId = userId,
        };

        await _productRepository.AddAsync(newProduct);
        return Ok(newProduct);
    }
    
    // PUT: api/products/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(short id, [FromBody] CreateProductDto productDto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null || product.Status == 0)
        {
            return NotFound(ProductNotFoundMessage);
        }

        // Validar que la categoría exista
        var category_val = await _categoryRepository.GetByIdAsync(productDto.CategoryId);
        if (category_val == null)
        {
            return BadRequest("La categoría seleccionada no existe.");
        }
        if (category_val.Status == 0)
        {
            return BadRequest("La categoría seleccionada está inactiva.");
        }

        product.Name = productDto.Name;
        product.Description = productDto.Description;
        product.CategoryId = productDto.CategoryId;
        product.TotalStock = productDto.TotalStock;

        await _productRepository.UpdateAsync(product);
        return Ok(product);
    }

    // DELETE: api/products/{id} (Borrado lógico - solo Admin)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(short id, [FromHeader] string userRole)
    {
        if (userRole != "Admin")
        {
            return Forbid("Acción no permitida. Solo los administradores pueden eliminar productos.");
        }

        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound(ProductNotFoundMessage);
        }

        await _productRepository.DeleteAsync(id);
        return Ok("Producto eliminado correctamente.");
    }

    // PUT: api/products/{id}/activate (solo Admin)
    [HttpPut("{id}/activate")]
    public async Task<IActionResult> ActivateProduct(short id, [FromHeader] string userRole)
    {
        if (userRole != "Admin")
        {
            return Forbid("Solo los administradores pueden activar productos.");
        }

        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound(ProductNotFoundMessage);
        }

        product.Status = 1;
        product.ModificationDate = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);
        return Ok("Producto activado correctamente.");
    }
}