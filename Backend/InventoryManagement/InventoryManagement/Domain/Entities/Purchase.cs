using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryManagement.Domain.Entities;

[Table("Purchase")]
public class Purchase : AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [JsonRequired]
    [Column("total_purchase", TypeName = "decimal(10, 2)")]
    public decimal TotalPurchase { get; set; }

    [Required]
    [JsonRequired]
    [ForeignKey("Supplier")]
    [Column("id_supplier")]
    public short SupplierId { get; set; }
    public required virtual Supplier Supplier { get; set; }

    // Propiedad de navegación: una compra tiene muchos detalles
    public required virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }
}