using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Google.Protobuf.WellKnownTypes;

namespace Backend.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string Email { get; set; } = string.Empty;

    [Required] [MaxLength(130)] public byte[] PasswordHash { get; set; }
    [Required] [MaxLength(50)] public string FirstName { get; set; } = string.Empty;
    [Required] [MaxLength(50)] public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public byte[]? RememberMe { get; set; }
    public byte[]? RefreshToken { get; set; }
    public ICollection<UserPost> UserPosts { get; set; }
    public ICollection<Like> Likes { get; set; }
    public ICollection<Picture> Pictures { get; set; }

    [Required]
    [MaxLength(7)]
    [Column(TypeName = "varchar(7)")]
    public string Gender { get; set; } = string.Empty;
}

public class UserViewModel
{
    public int Id { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
  
    public  UserViewModel( int? id, string firstName, string lastName)
    {
        if (id == null)
        {
            return;
        }

        this.Id = (int)id;
        this.FirstName = firstName;
        this.LastName = lastName;
    }
  

}