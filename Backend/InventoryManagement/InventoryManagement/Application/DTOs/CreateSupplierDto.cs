namespace InventoryManagement.Application.DTOs;
using System.ComponentModel.DataAnnotations;

public class CreateSupplierDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(31, MinimumLength = 4, ErrorMessage = "El nombre debe tener entre 4 y 31 caracteres")]
    [RegularExpression("^(?!\\d+$).*$", ErrorMessage = "El nombre no puede ser solo números")]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "El NIT es obligatorio")]
    [StringLength(12, MinimumLength = 7, ErrorMessage = "El NIT debe tener entre 7 y 12 caracteres")]
    public string? Nit { get; set; }
    [Required(ErrorMessage = "La dirección es obligatoria")]
    [StringLength(100, MinimumLength = 4, ErrorMessage = "La dirección debe tener entre 4 y 100 caracteres")]
    public string? Address { get; set; }
    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [StringLength(25, ErrorMessage = "El teléfono (incluyendo código) no puede exceder 25 caracteres")]
    public string? Phone { get; set; }
    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
    [StringLength(30, MinimumLength = 5, ErrorMessage = "El correo debe tener entre 5 y 30 caracteres")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "El nombre de contacto es obligatorio")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre de contacto debe tener entre 2 y 50 caracteres")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre de contacto solo puede contener letras y espacios")]
    public string? ContactName { get; set; }
}