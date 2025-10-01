namespace InventoryManagement.Application.DTOs;

public class CreateUserDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? SecondLastName { get; set; }
    public required string Role { get; set; }
}