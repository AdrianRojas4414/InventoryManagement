using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Entities;

[Table("Supplier")]
public class Supplier : AuditableEntity
{
    [Key]
    public short Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(20)]
    public string? Nit { get; set; }

    public string? Address { get; set; }

    [MaxLength(25)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(100)]
    public string? ContactName { get; set; }
    
    // Propiedad de navegación: un proveedor ofrece muchos productos
    public virtual ICollection<SupplierProduct> SupplierProducts { get; set; }
}