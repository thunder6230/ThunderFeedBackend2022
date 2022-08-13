namespace Backend.Models;

public class Reply
{
    public int Id { get; set; }
    public string Body { get; set; } = string.Empty;
    public User User { get; set; }
    public ICollection<Like> Likes { get; set; }
    public Comment Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ReplyViewModel{
    public int Id { get; set; }
    public string Body { get; set; } = string.Empty;
    public UserViewModel User { get; set; }
    public List<ReplyLikeViewModel> Likes { get; set; }
    public int CommentId { get; set; }
    public DateTime CreatedAt { get; set; }
    private ApplicationDbContext _context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());

    public ReplyViewModel(Reply reply)
    {
        Id = reply.Id;
        Body = reply.Body;
        User = new UserViewModel(reply.Id, reply.User.FirstName, reply.User.LastName);
        CommentId = reply.Comment.Id;
        CreatedAt = reply.CreatedAt;
        Likes = GetLikes();
    }
    public List<ReplyLikeViewModel> GetLikes()
    {
        var replies = _context.Likes.Where(l => l.Reply.Id == Id).ToList();
        var viewReplies = new List<ReplyLikeViewModel>();
        foreach (var like in replies)
        {
            viewReplies.Add(new ReplyLikeViewModel(like));
        }

        return viewReplies;
    }
}