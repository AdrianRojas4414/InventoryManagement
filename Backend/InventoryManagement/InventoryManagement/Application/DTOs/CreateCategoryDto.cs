namespace InventoryManagement.Application.DTOs;

public class CreateCategoryDto
{
    public required string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}