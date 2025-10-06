using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Entities;

[Table("Purchase")]
public class Purchase : AuditableEntity
{
    [Key]
    public int Id { get; set; } 

    [Column(TypeName = "decimal(10, 2)")]
    public decimal TotalPurchase { get; set; }

    [ForeignKey("Supplier")]
    public short SupplierId { get; set; }
    [ForeignKey("User")]
    public short UserId { get; set; }
    public required virtual Supplier Supplier { get; set; }

    // Propiedad de navegación: una compra tiene muchos detalles
    public required virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }
}