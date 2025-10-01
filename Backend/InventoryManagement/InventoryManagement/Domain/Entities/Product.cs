using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Entities;

[Table("Product")]
public class Product : AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)] 
    public short Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    public string? Description { get; set; }

    public short TotalStock { get; set; }

    // Relación con Category
    [ForeignKey("Category")]
    public byte CategoryId { get; set; }
    public required virtual Category Category { get; set; }
    
    // Propiedad de navegación: un producto es ofrecido por muchos proveedores
    public required virtual ICollection<SupplierProduct> SupplierProducts { get; set; }

    // Propiedad de navegación: un producto está en muchos detalles de compra
    public required virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }
}