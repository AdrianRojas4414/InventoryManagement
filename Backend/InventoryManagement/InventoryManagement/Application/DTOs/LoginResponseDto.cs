namespace InventoryManagement.Application.DTOs;

public class LoginResponseDto
{
    public short Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}