using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.DTOs;
using System.Globalization;

namespace InventoryManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly IPurchaseRepository _purchaseRepository;

    public PurchasesController(IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository;
    }

    // POST: api/purchases -> Registrar nueva compra
    [HttpPost]
    public async Task<IActionResult> CreatePurchase(
        [FromBody] CreatePurchaseDto purchaseDto, 
        [FromHeader] short userId)
    {
        try
        {
            await _purchaseRepository.AddAsync(purchaseDto, userId);
            return Ok(new { message = "Compra registrada exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    // GET: api/purchases -> Listar todas las compras (solo Admin)
    [HttpGet]
    public async Task<IActionResult> GetAllPurchases([FromHeader] string userRole)
    {
        if (userRole != "Admin")
        {
            return Forbid("Acción no permitida. Solo los administradores pueden ver todas las compras.");
        }

        var purchases = await _purchaseRepository.GetAllAsync();
        var response = MapPurchasesToResponse(purchases);
        return Ok(response);
    }

    // GET: api/purchases/user -> Listar compras del usuario autenticado
    [HttpGet("user")]
    public async Task<IActionResult> GetUserPurchases([FromHeader] short userId)
    {
        var purchases = await _purchaseRepository.GetByUserIdAsync(userId);
        var response = MapPurchasesToResponse(purchases);
        return Ok(response);
    }

    // GET: api/purchases/{id} -> Obtener una compra específica
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPurchaseById(int id, [FromHeader] short userId, [FromHeader] string userRole)
    {
        var purchase = await _purchaseRepository.GetByIdAsync(id);
        
        if (purchase == null)
        {
            return NotFound("Compra no encontrada.");
        }

        // Validar que el usuario pueda ver esta compra
        if (userRole != "Admin" && purchase.CreatedByUserId != userId)
        {
            return Forbid("No tiene permisos para ver esta compra.");
        }

        var response = MapPurchaseToResponse(purchase);
        return Ok(response);
    }

    // PUT: api/purchases/{id} -> Actualizar compra (solo Admin)
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePurchase(int id, [FromHeader] string userRole)
    {
        if (userRole != "Admin")
        {
            return Forbid("Acción no permitida. Solo los administradores pueden actualizar compras.");
        }

        var purchase = await _purchaseRepository.GetByIdAsync(id);
        if (purchase == null)
        {
            return NotFound("Compra no encontrada.");
        }

        try
        {
            await _purchaseRepository.UpdateAsync(purchase);
            return Ok(new { message = "Compra actualizada exitosamente", purchase });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error al actualizar la compra", details = ex.Message });
        }
    }

    // Método helper para mapear una lista de compras
    private List<PurchaseResponseDto> MapPurchasesToResponse(List<Domain.Entities.Purchase> purchases)
    {
        return purchases.Select(p => MapPurchaseToResponse(p)).ToList();
    }

    // Método helper para mapear una compra individual
    private PurchaseResponseDto MapPurchaseToResponse(Domain.Entities.Purchase purchase)
    {
        return new PurchaseResponseDto
        {
            Id = purchase.Id,
            Fecha = purchase.CreationDate.ToString("dd/MM/yyyy"),
            Proveedor = purchase.Supplier?.Name ?? "Sin proveedor",
            Total = $"{purchase.TotalPurchase:F2} BOB",
            Expanded = false,
            Detalles = purchase.PurchaseDetails?.Select(pd => new PurchaseDetailResponseDto
            {
                Producto = pd.Product?.Name ?? "Sin nombre",
                Cantidad = pd.Quantity,
                PrecioUnitario = pd.UnitPrice
            }).ToList() ?? new List<PurchaseDetailResponseDto>()
        };
    }
}