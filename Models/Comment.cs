namespace Backend.Models;

public class Comment
{
    public int Id { get; set; }
    public string Body { get; set; } = string.Empty;
    public User User { get; set; }
    public ICollection<Like> Likes { get; set; }
    public UserPost UserPost { get; set; }
    public ICollection<Picture> Pictures { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<Reply> Replies { get; set; }
}