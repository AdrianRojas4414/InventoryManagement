using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryManagement.Domain.Entities;

[Table("User")]
public class User : AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public short Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("username")]
    public string Username { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    [Column("first_name")]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    [Column("last_name")]
    public string LastName { get; set; } = null!;

    [MaxLength(50)]
    [Column("second_last_name")]
    public string? SecondLastName { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("role")]
    public string Role { get; set; } = null!;
}