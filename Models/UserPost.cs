using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;

namespace Backend.Models;

public class UserPost
{
    public int Id { get; set; }
    public string Body { get; set; } = string.Empty;

    public User User { get; set; }
   
    public int? UserToId { get; set; }
    public ICollection<Like> Likes { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<Picture> Pictures { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
}

public class UserPostViewModel
{
    public int Id { get; set; }
    public string Body { get; set; } = string.Empty;

    public UserViewModel User { get; set; }
    public UserViewModel? UserTo { get; set; }
    public List<PostLikeViewModel> Likes { get; set; }
    public List<CommentViewModel> Comments { get; set; }
    public List<PictureViewModel> Pictures { get; set; }
    
    private ApplicationDbContext _context { get; set; }
    public DateTime CreatedAt { get; set; }

    public UserPostViewModel(UserPost post, ApplicationDbContext context)
    {
        // _context = context;
        // var post = _context.UserPosts.First(p => p.Id == id);
        
        Id = post.Id;
        User = new UserViewModel(post.User);
        UserTo = new UserViewModel(post.UserToId);
        Body = post.Body;
        CreatedAt = post.CreatedAt;
        Likes = GetLikes();
        Comments = GetComments();
        Pictures = GetPictures();
    }
    public List<CommentViewModel> GetComments()
    {
        var comments = _context.Comments.Where(l => l.UserPost.Id == Id).ToList();
        var viewComments = new List<CommentViewModel>();
        foreach (var comment in comments)
        {
            viewComments.Add(new CommentViewModel(comment));
        }

        return viewComments;
    }
    public List<PostLikeViewModel> GetLikes()
    {
        var likes = _context.Likes.Where(l => l.UserPost.Id == Id).ToList();
        var viewLikes = new List<PostLikeViewModel>();
        foreach (var like in likes)
        {
            viewLikes.Add(new PostLikeViewModel(like));
        }

        return viewLikes;
    }
    public List<PictureViewModel> GetPictures()
    {
        var pictures = _context.Pictures.Where(l => l.UserPost.Id == Id).ToList();
        var viewPictures = new List<PictureViewModel>();
        foreach (var picture in pictures)
        {
            viewPictures.Add(new PictureViewModel(picture));
        }

        return viewPictures;
    }
     
}




