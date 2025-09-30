namespace InventoryManagement.Application.DTOs;

public class CreateUserDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? SecondLastName { get; set; }
    public string Role { get; set; }
}