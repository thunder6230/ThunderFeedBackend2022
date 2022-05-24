namespace Backend.Models;

public class UpdatePostRequest
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Body { get; set; }
}