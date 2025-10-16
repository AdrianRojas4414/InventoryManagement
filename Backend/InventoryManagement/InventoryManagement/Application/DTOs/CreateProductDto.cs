using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs;

public class CreateProductDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
    public required string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripci�n no puede exceder 500 caracteres")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "La categor�a es requerida")]
    [Range(1, short.MaxValue, ErrorMessage = "Debe seleccionar una categor�a v�lida")]
    public required short CategoryId { get; set; }

    [Range(0, short.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public short TotalStock { get; set; } = 0;
}