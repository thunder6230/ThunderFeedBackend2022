using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace Backend.Models;

public class UserPost
{
    public int Id { get; set; }
    public string Body { get; set; } = string.Empty;

    public User User { get; set; }
    // public Picture Picture { get; set; }
    public ICollection<Like> Likes { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}