using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Options;

namespace Backend.Models;

public class Like
{
    public int Id { get; set; }
    public User User { get; set; }
    
    public UserPost? UserPost { get; set; }
    public Comment? Comment { get; set; }
    public Reply? Reply { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PostLikeViewModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int UserPostId { get; set; }
    
}
public class CommentLikeViewModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    public int CommentId { get; set; }
    
}
public class ReplyLikeViewModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ReplyId { get; set; }
    
}