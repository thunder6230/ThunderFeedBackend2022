namespace Backend.Models;
using System.ComponentModel.DataAnnotations;
public class RegisterModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Password_Confirm { get; set; } = string.Empty;
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;
}