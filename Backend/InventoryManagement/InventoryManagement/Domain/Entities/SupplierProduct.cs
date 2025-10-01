using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Entities;

[Table("Supplier_Product")]
public class SupplierProduct : AuditableEntity
{
    public short SupplierId { get; set; }
    public short ProductId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal ProductCost { get; set; }

    // Propiedades de navegación
    public required virtual Supplier Supplier { get; set; }
    public required virtual Product Product { get; set; }
}