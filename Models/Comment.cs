namespace Backend.Models;

public class Comment
{
    public int Id { get; set; }
    public string Body { get; set; } = string.Empty;
    public User User { get; set; }
    public List<Like> Likes { get; set; }
    public UserPost UserPost { get; set; }
    public DateTime CreatedAt { get; set; }
    
}