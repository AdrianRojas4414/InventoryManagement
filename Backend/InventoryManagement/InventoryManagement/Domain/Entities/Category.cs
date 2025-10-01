using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Entities;

[Table("Category")]
public class Category : AuditableEntity
{
    [Key]
    public byte Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string? Name { get; set; }

    public string? Description { get; set; }

    // Propiedad de navegación: una categoría tiene muchos productos
    public required virtual ICollection<Product> Products { get; set; }
}