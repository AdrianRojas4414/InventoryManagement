using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Entities;

[Table("Purchase_Detail")]
public class PurchaseDetail
{
    [Column("id_purchase")]
    public int PurchaseId { get; set; }
    [Column("id_product")]
    public short ProductId { get; set; }
    [Column("quantity")]
    public short Quantity { get; set; }

    [Column("unit_price", TypeName = "decimal(10, 2)")]
    public decimal UnitPrice { get; set; }

    // Propiedades de navegación
    public required virtual Purchase Purchase { get; set; }
    public required virtual Product Product { get; set; }
}