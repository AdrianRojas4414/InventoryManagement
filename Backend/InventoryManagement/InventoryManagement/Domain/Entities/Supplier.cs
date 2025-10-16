using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryManagement.Domain.Entities;

[Table("Supplier")]
public class Supplier : AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public short Id { get; set; } // NOSONAR

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = null!;

    [MaxLength(20)]
    [Column("nit")]
    public string? Nit { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [MaxLength(25)]
    [Column("phone")]
    public string? Phone { get; set; }

    [MaxLength(100)]
    [Column("email")]
    public string? Email { get; set; }

    [MaxLength(100)]
    [Column("contact_name")]
    public string? ContactName { get; set; }
    
    // Propiedad de navegación: un proveedor ofrece muchos productos
    public virtual ICollection<SupplierProduct> SupplierProducts { get; set; } = new List<SupplierProduct>();
}