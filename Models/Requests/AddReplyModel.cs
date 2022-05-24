namespace Backend.Models;

public class AddReplyModel
{
    public int CommentId { get; set; }
    public int UserId { get; set; }
    public string Body { get; set; } = string.Empty;
    public string? File { get; set; } 
    // public Image Image { get; set; }
    
}