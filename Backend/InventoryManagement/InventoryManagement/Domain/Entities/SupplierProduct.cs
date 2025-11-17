using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Entities;

[Table("Supplier_Product")]
public class SupplierProduct : AuditableEntity
{
    [Column("id_supplier")]
    [ForeignKey("Supplier")]
    public short SupplierId { get; set; }
    [Column("id_product")]
    [ForeignKey("Product")]
    public short ProductId { get; set; }

    [Column("product_cost", TypeName = "decimal(10, 2)")]
    public decimal ProductCost { get; set; }

    // Propiedades de navegación
    public required virtual Supplier Supplier { get; set; }
    public required virtual Product Product { get; set; }
}