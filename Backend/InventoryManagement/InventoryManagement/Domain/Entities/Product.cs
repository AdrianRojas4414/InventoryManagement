using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Entities;

[Table("Product")]
public class Product : AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("id")]
    public short Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("total_stock")]
    public short TotalStock { get; set; } = 0;

    [ForeignKey("Category")]
    [Column("id_category")]
    public required short CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    // Propiedad de navegación: un producto es ofrecido por muchos proveedores
    public virtual ICollection<SupplierProduct> SupplierProducts { get; set; } = new List<SupplierProduct>();

    // Propiedad de navegación: un producto está en muchos detalles de compra
    public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();
}