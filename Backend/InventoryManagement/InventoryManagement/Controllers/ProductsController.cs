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
    private const string AdminRole = "Admin";

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    // GET: api/products -> Criterio: Visualizar todos los productos
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productRepository.GetAllAsync();
        return Ok(products);
    }

    // POST: api/products -> Criterio: Registrar nuevos productos con código de barras
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto, [FromHeader] short userId)
    {
        var newProduct = new Product
        {
            Id = productDto.Id,
            Name = productDto.Name,
            Description = productDto.Description,
            CategoryId = productDto.CategoryId,
            TotalStock = productDto.TotalStock,
            Status = 1,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        await _productRepository.AddAsync(newProduct);
        return Ok(newProduct);
    }
}