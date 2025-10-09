using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseDetailsController : ControllerBase
{
    private readonly IPurchaseDetailRepository _purchaseDetailRepository;

    public PurchaseDetailsController(IPurchaseDetailRepository purchaseDetailRepository)
    {
        _purchaseDetailRepository = purchaseDetailRepository;
    }

    // GET: api/purchasedetails -> Obtener todos los detalles (solo Admin)
    [HttpGet]
    public async Task<IActionResult> GetAllPurchaseDetails([FromHeader] string userRole)
    {
        if (userRole != "Admin")
        {
            return Forbid("Acción no permitida. Solo los administradores pueden ver todos los detalles.");
        }

        var details = await _purchaseDetailRepository.GetAllAsync();
        return Ok(details);
    }

    // GET: api/purchasedetails/purchase/{purchaseId} -> Obtener detalles de una compra específica
    [HttpGet("purchase/{purchaseId}")]
    public async Task<IActionResult> GetDetailsByPurchaseId(int purchaseId)
    {
        var details = await _purchaseDetailRepository.GetByPurchaseIdAsync(purchaseId);
        
        if (details == null || details.Count == 0)
        {
            return NotFound("No se encontraron detalles para esta compra.");
        }

        return Ok(details);
    }

    // POST: api/purchasedetails -> Agregar detalle (generalmente no se usa independientemente)
    [HttpPost]
    public async Task<IActionResult> AddPurchaseDetail(
        [FromBody] PurchaseDetail purchaseDetail,
        [FromHeader] string userRole)
    {
        if (userRole != "Admin")
        {
            return Forbid("Acción no permitida. Solo los administradores pueden agregar detalles.");
        }

        try
        {
            await _purchaseDetailRepository.AddAsync(purchaseDetail);
            return Ok(new { message = "Detalle agregado exitosamente", purchaseDetail });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error al agregar detalle", details = ex.Message });
        }
    }

    // PUT: api/purchasedetails -> Actualizar detalle (solo Admin)
    [HttpPut]
    public async Task<IActionResult> UpdatePurchaseDetail(
        [FromBody] PurchaseDetail purchaseDetail,
        [FromHeader] string userRole)
    {
        if (userRole != "Admin")
        {
            return Forbid("Acción no permitida. Solo los administradores pueden actualizar detalles.");
        }

        try
        {
            await _purchaseDetailRepository.UpdateAsync(purchaseDetail);
            return Ok(new { message = "Detalle actualizado exitosamente", purchaseDetail });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error al actualizar detalle", details = ex.Message });
        }
    }
}