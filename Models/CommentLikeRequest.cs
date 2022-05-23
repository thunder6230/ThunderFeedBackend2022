namespace Backend.Models;

public class CommentLikeRequest
{
    public int commentId { get; set; }
    public int userId { get; set; }
}