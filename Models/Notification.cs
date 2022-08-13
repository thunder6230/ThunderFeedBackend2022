using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Options;

namespace Backend.Models;

public class Notification
{
    public int Id { get; set; }
    public User User { get; set; }
    public int UserFromId { get; set; }
    public string Type { get; set; }
    public UserPost? UserPost { get; set; }
    public Comment? Comment { get; set; }
    public Reply? Reply { get; set; }
    public bool IsUnread { get; set; }
    private ApplicationDbContext _context { get; set; }
    public DateTime CreatedAt { get; set; }
    
}

public class PostLikeNotification : Notification
{
    public UserPost UserPost { get; set; }

    public PostLikeNotification(User user, int userFromId, UserPost post)
    {
        this.User = user;
        this.UserFromId = userFromId;
        this.Type = "LIKE_USERPOST";
        this.UserPost = post;
        this.CreatedAt = DateTime.Now;
        this.IsUnread = true;
    }
} 
public class PostToUserNotification : Notification
{
    public UserPost UserPost { get; set; }

    public PostToUserNotification(User user, int userFromId, UserPost post)
    {
        this.User = user;
        this.UserFromId = userFromId;
        this.Type = "POST_TO_USER";
        this.UserPost = post;
        this.CreatedAt = DateTime.Now;
        this.IsUnread = true;
    }
}
public class CommentLikeNotification : Notification
{
    public UserPost UserPost { get; set; }
    public Comment Comment { get; set; }

    public CommentLikeNotification(User user, int userFromId, UserPost post, Comment comment)
    {
        this.User = user;
        this.UserFromId = userFromId;
        this.Type = "LIKE_USERPOST";
        this.UserPost = post;
        this.Comment = comment;
        this.CreatedAt = DateTime.Now;
        this.IsUnread = true;
    }
} 
public class ReplyLikeNotification : Notification
{
    public UserPost UserPost { get; set; }
    public Comment Comment { get; set; }
    public Reply Reply { get; set; }

    public ReplyLikeNotification(User user, int userFromId, UserPost post, Comment comment, Reply reply)
    {
        this.User = user;
        this.UserFromId = userFromId;
        this.Type = "LIKE_USERPOST";
        this.UserPost = post;
        this.Comment = comment;
        this.CreatedAt = DateTime.Now;
        this.IsUnread = true;
        this.Reply = reply;
    }
} 
public class CommentReplyNotification : Notification
{
    public UserPost UserPost { get; set; }
    public Comment Comment { get; set; }
    public Reply Reply { get; set; }

    public CommentReplyNotification(User user, int userFromId, UserPost post, Comment comment, Reply reply)
    {
        this.User = user;
        this.UserFromId = userFromId;
        this.Type = "POST_REPLY";
        this.UserPost = post;
        this.CreatedAt = DateTime.Now;
        this.IsUnread = true;
        Comment = comment;
        this.Reply = reply;
    }
} 

public class NotificationViewModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserViewModel UserFrom { get; set; }
    public string Type { get; set; }
    public int UserPostId { get; set; }
    public int? CommentId { get; set; }
    public int? ReplyId { get; set; }
    public bool? IsUnread { get; set; }
    

}
