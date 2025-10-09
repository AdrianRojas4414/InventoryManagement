namespace InventoryManagement.Application.DTOs;

// DTO para la respuesta de lista de compras
public class PurchaseResponseDto
{
    public int Id { get; set; }
    public string Fecha { get; set; } = string.Empty;
    public string Proveedor { get; set; } = string.Empty;
    public string Total { get; set; } = string.Empty;
    public bool Expanded { get; set; } = false;
    public List<PurchaseDetailResponseDto> Detalles { get; set; } = new();
}

// DTO para los detalles de compra
public class PurchaseDetailResponseDto
{
    public string Producto { get; set; } = string.Empty;
    public short Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}