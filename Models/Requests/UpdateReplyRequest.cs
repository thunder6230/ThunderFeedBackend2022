namespace Backend.Models;

public class UpdateReplyRequest
{
    public int ReplyId { get; set; }
    public int UserId { get; set; }
    public string Body { get; set; }
}