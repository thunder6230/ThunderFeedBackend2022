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
    public DateTime UpdatedAt { get; set; }
    
}

public class UserPostViewModel
{
    public int Id { get; set; }
    public string Body { get; set; } = string.Empty;

    public UserViewModel User { get; set; }
    public UserViewModel UserTo { get; set; }
    public IEnumerable<PostLikeViewModel> Likes { get; set; }
    public IEnumerable<CommentViewModel> Comments { get; set; }
    public IEnumerable<PictureViewModel> Pictures { get; set; }
     public DateTime CreatedAt { get; set; }
}




