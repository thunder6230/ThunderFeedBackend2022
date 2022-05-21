using System.Runtime.CompilerServices;

namespace Backend.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; }


    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public byte[]? RememberMe { get; set; }
    public ICollection<UserPost> UserPosts { get; set; }
    public ICollection<Like> Likes { get; set; }

    public string Gender { get; set; } = string.Empty;
    
    public string FullName => LastName + " " + FirstName;
}
