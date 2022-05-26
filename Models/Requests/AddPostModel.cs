namespace Backend.Models;

public class AddPostModel
{
    public string Body { get; set; } = string.Empty;
    public int UserId { get; set; }
    public IFormFileCollection? File { get; set; }
    // public Image Image { get; set; }
    
}