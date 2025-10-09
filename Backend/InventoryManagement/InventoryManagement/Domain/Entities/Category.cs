using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryManagement.Domain.Entities;

[Table("Category")]
public class Category : AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    [Column("id")]
    public short Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public required string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    // Propiedad de navegación: una categoría tiene muchos productos
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}