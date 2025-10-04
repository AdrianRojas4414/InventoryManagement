namespace InventoryManagement.Application.DTOs;

public class CreatePurchaseDetailDto
{
    public required ushort SupplierProductId { get; set; }
    public required short Quantity { get; set; }
    public required decimal UnitPrice { get; set; }
}