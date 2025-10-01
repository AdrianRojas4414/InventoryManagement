namespace InventoryManagement.Application.DTOs;

public class CreateSupplierDto
{
    public string Name { get; set; } = string.Empty;
    public string? Nit { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ContactName { get; set; }
}