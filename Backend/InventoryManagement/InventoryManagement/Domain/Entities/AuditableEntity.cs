namespace InventoryManagement.Domain.Entities;

public abstract class AuditableEntity
{
    // Foreign Key para el usuario que creó el registro
    public short CreatedByUserId { get; set; }
    
    public DateTime CreationDate { get; set; }
    
    public DateTime ModificationDate { get; set; }
    
    // 1: Activo, 0: Inactivo
    public byte Status { get; set; }
    
    // Propiedad de navegación hacia el usuario creador
    public virtual User CreatedByUser { get; set; }
}