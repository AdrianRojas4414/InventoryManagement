namespace InventoryManagement.Application.DTOs;

public class CreateProductDto
{
    public required short Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public required short CategoryId { get; set; }
    public short TotalStock { get; set; } = 0;
}