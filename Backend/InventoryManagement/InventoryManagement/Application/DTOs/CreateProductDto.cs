using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs;

public class CreateProductDto
{
    [Required(ErrorMessage = "El código serial es requerido")]
    [Range(1, short.MaxValue, ErrorMessage = "El código serial debe ser un número positivo")]
    public required short SerialCode { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
    public required string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "La categor�a es requerida")]
    [Range(1, short.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida")]
    public required short CategoryId { get; set; }

    [Range(0, short.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public short TotalStock { get; set; } = 0;

    
}