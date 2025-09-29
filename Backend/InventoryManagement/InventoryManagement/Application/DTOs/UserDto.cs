namespace InventoryManagement.Application.DTOs
{
    public class UserDto
    {
        public ushort Id { get; set; }
        public ushort CreatedByUserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Password { get; set; } // Only for create/update, never returned in GET
        public string FirstLastName { get; set; } = string.Empty;
        public string SecondLastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public bool IsActive { get; set; }
    }
}
