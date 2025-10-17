using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Application.DTOs;
using Microsoft.EntityFrameworkCore; 

namespace InventoryManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierRepository _supplierRepository;
    private const string SupplierNotFoundMessage = "Proveedor no encontrado.";


    public SuppliersController(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    // GET: api/suppliers -> Criterio: Visualizar todos los proveedores
    [HttpGet]
    public async Task<IActionResult> GetAllSuppliers()
    {
        var suppliers = await _supplierRepository.GetAllAsync();
        return Ok(suppliers);
    }

    // GET: api/suppliers/{id} -> Criterio: Visualizar un proveedor
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSupplierById(short id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null || supplier.Status == 0)
        {
            return NotFound(SupplierNotFoundMessage);
        }
        return Ok(supplier);
    }

    // POST: api/suppliers -> Criterio: Registrar nuevos proveedores
    [HttpPost]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierDto supplierDto, [FromHeader] short userId)
    {
        var newSupplier = new Supplier
        {
            Name = supplierDto.Name,
            Nit = supplierDto.Nit,
            Address = supplierDto.Address,
            Phone = supplierDto.Phone,
            Email = supplierDto.Email,
            ContactName = supplierDto.ContactName,
            Status = 1,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
            CreatedByUserId = userId 
        };

        await _supplierRepository.AddAsync(newSupplier);
        return CreatedAtAction(nameof(GetSupplierById), new { id = newSupplier.Id }, newSupplier);
    }

    // PUT: api/suppliers/{id} -> Criterio: Editar información de proveedores
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSupplier(short id, [FromBody] UpdateSupplierDto supplierDto)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null || supplier.Status == 0)
        {
            return NotFound(SupplierNotFoundMessage);
        }

        supplier.Name = supplierDto.Name;
        supplier.Nit = supplierDto.Nit;
        supplier.Address = supplierDto.Address;
        supplier.Phone = supplierDto.Phone;
        supplier.Email = supplierDto.Email;
        supplier.ContactName = supplierDto.ContactName;
        supplier.ModificationDate = DateTime.UtcNow;

        await _supplierRepository.UpdateAsync(supplier);
        return Ok(supplier);
    }

    // DELETE: api/suppliers/{id} -> Criterio: Dar de baja a los proveedores (solo Admin)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeactivateSupplier(short id, [FromHeader] string userRole)
    {
        if (userRole != "Admin")
        {
            return BadRequest("Acción no permitida. Solo los administradores pueden dar de baja a los proveedores.");
        }

        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null)
        {
            return NotFound(SupplierNotFoundMessage);
        }

        supplier.Status = 0; 
        supplier.ModificationDate = DateTime.UtcNow;

        await _supplierRepository.UpdateAsync(supplier);
        return Ok("Proveedor dado de baja correctamente.");
    }

    [HttpPut("{id}/activate")]
    public async Task<IActionResult> ActivateSupplier(short id, [FromHeader] string userRole)
    {
        if (userRole != "Admin")
            return BadRequest("Solo los administradores pueden habilitar proveedores.");

        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null)
            return NotFound(SupplierNotFoundMessage);

        supplier.Status = 1; // habilitado
        supplier.ModificationDate = DateTime.UtcNow;

        await _supplierRepository.UpdateAsync(supplier);
        return Ok("Proveedor habilitado correctamente.");
    }

}