using System.ComponentModel.DataAnnotations.Schema;
namespace InventoryManagement.Domain.Entities;

public abstract class AuditableEntity
{
    [Column("id_user_creation")]
    public short? CreatedByUserId { get; set; }

    [Column("creation_date")] 
    public DateTime CreationDate { get; set; }

    [Column("modification_date")] 
    public DateTime ModificationDate { get; set; }

    [Column("status")]
    public byte Status { get; set; }

    public virtual User? CreatedByUser { get; set; } = null!;
}