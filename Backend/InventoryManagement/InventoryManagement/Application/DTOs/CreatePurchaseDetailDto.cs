namespace InventoryManagement.Application.DTOs;

public class CreatePurchaseDetailDto
{
    public required short ProductId { get; set; }
    public required short Quantity { get; set; }
    public required decimal UnitPrice { get; set; }
}