using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace InventoryManagement.Domain.Entities;

public abstract class AuditableEntity
{
    [Column("id_user_creation")]
    public short? CreatedByUserId { get; set; }

    [JsonRequired]
    [Column("creation_date")] 
    public DateTime CreationDate { get; set; }

    [JsonRequired]
    [Column("modification_date")] 
    public DateTime ModificationDate { get; set; }

    [JsonRequired]
    [Column("status")]
    public byte Status { get; set; }

    public virtual User? CreatedByUser { get; set; } = null!;
}