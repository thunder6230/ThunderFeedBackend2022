using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Options;

namespace Backend.Models;

public class Like
{
    public int Id { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserPost? UserPost { get; set; }
    public Comment? Comment { get; set; }
    public Reply? Reply { get; set; }
    public Picture? Picture { get; set; }
    
}

public class PostLike : Like
{
    public UserPost UserPost { get; set; }

    public PostLike(User user, UserPost post)
    {
        CreatedAt = DateTime.Now;
        User = user;
        UserPost = post;
    }
}public class PictureLike : Like
{
    public Picture Picture { get; set; }

    public PictureLike(User user, Picture picture)
    {
        CreatedAt = DateTime.Now;
        User = user;
        Picture = picture;
    }
}

public class CommentLike : Like
{
    
    public Comment Comment { get; set; }
    public CommentLike(User user, Comment comment)
    {
        CreatedAt = DateTime.Now;
        User = user;
        Comment = comment;
    }
}
public class ReplyLike : Like
{
    public Reply Reply { get; set; }
    public ReplyLike(User user, Reply reply)
    {
        CreatedAt = DateTime.Now;
        User = user;
        Reply = reply;
    }
}


public class LikeViewModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
}
public class PostLikeViewModel : LikeViewModel{
    public int UserPostId { get; set; }
    public PostLikeViewModel(Like like) 
    {
        Id = like.Id;
        UserId = like.User.Id;
        UserPostId = like.UserPost.Id;
    }
    
}
public class CommentLikeViewModel : LikeViewModel
{
    public int CommentId { get; set; }

    public CommentLikeViewModel(Like like)
    {
        Id = like.Id;
        UserId = like.User.Id;
        CommentId = like.Comment.Id;
    }
}
public class ReplyLikeViewModel : LikeViewModel
{
  public int ReplyId { get; set; }
    public ReplyLikeViewModel(Like like)
    {
        Id = like.Id;
        UserId = like.User.Id;
        ReplyId = like.Reply.Id;
    }
    
}
public class PictureLikeViewModel : LikeViewModel
{
  public int PictureId { get; set; }
    public PictureLikeViewModel(Like like)
    {
        Id = like.Id;
        UserId = like.User.Id;
        PictureId = like.Picture.Id;
    }
    
}