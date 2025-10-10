using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryManagement.Domain.Entities;

[Table("Purchase_Detail")]
public class PurchaseDetail
{
    [JsonRequired]
    [Column("id_purchase")]
    public int PurchaseId { get; set; }

    [JsonRequired]
    [Column("id_product")]
    public short ProductId { get; set; }

    [JsonRequired]
    [Column("quantity")]
    public short Quantity { get; set; }

    [JsonRequired]
    [Column("unit_price", TypeName = "decimal(10, 2)")]
    public decimal UnitPrice { get; set; }

    // Propiedades de navegación
    public required virtual Purchase Purchase { get; set; } = null!;
    public required virtual Product Product { get; set; } = null!;
}