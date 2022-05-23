namespace Backend.Models;

public class AddCommentModel
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Body { get; set; } = string.Empty;
    public string? File { get; set; } 
    // public Image Image { get; set; }
    
}