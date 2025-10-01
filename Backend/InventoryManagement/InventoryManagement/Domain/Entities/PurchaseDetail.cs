using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Entities;

[Table("Purchase_Detail")]
public class PurchaseDetail
{
    public int PurchaseId { get; set; }
    public short ProductId { get; set; }

    public short Quantity { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal UnitPrice { get; set; }

    // Propiedades de navegación
    public required virtual Purchase Purchase { get; set; }
    public required virtual Product Product { get; set; }
}