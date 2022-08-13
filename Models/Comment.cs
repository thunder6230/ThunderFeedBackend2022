using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Query;
using MySql.Data.MySqlClient.Replication;

namespace Backend.Models;

public class Comment
{
    public int Id { get; set; }
    public string Body { get; set; } = string.Empty;
    public User User { get; set; }
    public ICollection<Like> Likes { get; set; }
    public UserPost UserPost { get; set; }
    public ICollection<Picture> Pictures { get; set; }
    private ApplicationDbContext _context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<Reply> Replies { get; set; }

    
    
}
public class CommentViewModel
{
    public int Id { get; set; }
    private ApplicationDbContext _context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());
    public string Body { get; set; } = string.Empty;
    public UserViewModel User { get; set; }
    public List<CommentLikeViewModel> Likes { get; set; }
    public int UserPostId { get; set; }
    public List<PictureViewModel> Pictures { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public List<ReplyViewModel> Replies { get; set; }
    public CommentViewModel(Comment comment, User user)
    {
        Id = comment.Id;
        Body = comment.Body;
        User = new UserViewModel(comment.User.Id,  user.FirstName,  user.LastName);
        Likes = GetLikes();
        Replies = GetReplies();
        Pictures = GetPictures();
    }

    public List<ReplyViewModel> GetReplies()
    {
        var replies = _context.Replies.Where(l => l.Comment.Id == Id).ToList();
        var viewReplies = new List<ReplyViewModel>();
        foreach (var reply in replies)
        {
            viewReplies.Add(new ReplyViewModel(reply));
        }

        return viewReplies;
    }
    public List<CommentLikeViewModel> GetLikes()
    {
        var likes = _context.Likes.Where(l => l.Comment.Id == Id).ToList();
        var viewLikes = new List<CommentLikeViewModel>();
        foreach (var like in likes)
        {
            viewLikes.Add(new CommentLikeViewModel(like));
        }

        return viewLikes;
    }
    public List<PictureViewModel> GetPictures()
    {
        var pictures = _context.Pictures.Where(l => l.Comment.Id == Id).ToList();
        var viewPictures = new List<PictureViewModel>();
        foreach (var picture in pictures)
        {
            viewPictures.Add(new PictureViewModel(picture));
        }

        return viewPictures;
    }
}