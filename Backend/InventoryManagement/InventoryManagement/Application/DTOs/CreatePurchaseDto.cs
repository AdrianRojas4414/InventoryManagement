namespace InventoryManagement.Application.DTOs;

public class CreatePurchaseDto
{
    public required short SupplierId { get; set; }
    public required List<CreatePurchaseDetailDto> PurchaseDetails { get; set; } = new();
}