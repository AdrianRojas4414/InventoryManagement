namespace InventoryManagement.Domain.Entities;

public class User
{
    #region Attributes
    ushort id;
    ushort id_user_creation;
    string username;
    string password_hash;
    string fisrt_last_name;
    string second_last_name;
    string role;
    DateTime creation_date;
    DateTime modification_date;
    bool status;
    #endregion
    #region Properties
    public ushort Id { get; set; }
    public ushort Id_user_creation { get; set; }
    public string Username { get; set; }
    public string Password_hash { get; set; }
    public string First_last_name { get; set; }
    public string Second_last_name { get; set; }
    public string Role { get; set; }
    public DateTime Creation_date { get; set; }
    public DateTime Modification_date { get; set; }
    public bool Status { get; set; }
    #endregion
    #region Constructors
    public User() { }
    public User(ushort id, ushort id_user_creation, string username, string password_hash, string fisrt_last_name, string second_last_name, string role, DateTime creation_date, DateTime modification_date, bool status)
    {
        Id = id;
        Id_user_creation = id_user_creation;
        Username = username;
        Password_hash = password_hash;
        First_last_name = fisrt_last_name;
        Second_last_name = second_last_name;
        Role = role;
        Creation_date = creation_date;
        Modification_date = modification_date;
        Status = status;
    }
    #endregion
}