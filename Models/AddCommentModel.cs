namespace Backend.Models;

public class AddCommentModel
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Body { get; set; } = string.Empty;
    // public Image Image { get; set; }
    
}