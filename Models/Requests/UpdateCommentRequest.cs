namespace Backend.Models;

public class UpdateCommentRequest
{
    public int CommentId { get; set; }
    public int UserId { get; set; }
    public string Body { get; set; }
}