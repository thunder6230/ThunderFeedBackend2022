using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class LikeController : Controller
{
    private readonly ApplicationDbContext _context;

    // DB Data Contect
    public LikeController(ApplicationDbContext context)
    {
        _context = context;
    }
    // GET
    [HttpGet("/{id}")]
    public async Task<ActionResult<Like>> GetLike(int id)
    {
        try
        {
            return Ok(await _context.Likes.FindAsync(id));
        }
        catch
        {
            return BadRequest("Like Not Found");
        }
    }
    [Authorize]
    [HttpPost("AddPost")]
    public async Task<ActionResult<Like>> AddPostLike(PostLikeRequest request)
    {
        var postQuery = _context.UserPosts.Where(p => p.Id == request.postId)
            .Include(p => p.User)
            .Include(p => p.Likes)
            .ThenInclude(l => l.User);
        if (!postQuery.Any()) return BadRequest("Post Not found");
        var dbUserPost = postQuery.First();
        //It will be changed to AuthUser
        var userQuery = _context.Users.Where(u => u.Id == request.userId);
        if(!userQuery.Any()) return BadRequest("User not Found");
        var userOfLike = userQuery.First();
        if(dbUserPost.Likes.Any(l => l.User.Id == request.userId)) return Unauthorized("You have already Liked this Post");
        
        var newPostLike = new PostLike(userOfLike, dbUserPost);
        _context.Likes.Add(newPostLike);
        await _context.SaveChangesAsync();

        var like = _context.Likes.Where(l => l.User.Id == request.userId && l.UserPost.Id == request.postId)
            .Select(like => new PostLikeViewModel(like))
            .First();
        if (dbUserPost.User.Id != userOfLike.Id)
        {
            var notification = new PostLikeNotification(dbUserPost.User, userOfLike.Id, dbUserPost);
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }
       
        return Ok(like);
      
    }
    [HttpPost("AddComment")]
    public async Task<ActionResult<CommentLike>> AddCommentLike(CommentLikeRequest request)
    {
        var commentQuery = _context.Comments.Where(c => c.Id == request.commentId)
            .Include(c => c.User)
            .Include(c => c.Likes)
            .ThenInclude(l => l.User);
        if (!commentQuery.Any()) return BadRequest("Post Not found");
        var dbComment = commentQuery.First();
        
        var userOfLike = _context.Users.First(u => u.Id == request.userId);

        var newCommentLike = new CommentLike(userOfLike, dbComment);
        _context.Likes.Add(newCommentLike);
        
        await _context.SaveChangesAsync();
        
        var like = _context.Likes.Where(l => l.User.Id == request.userId && l.Comment.Id == request.commentId)
            .Select(like => new CommentLikeViewModel(like)).First();
        
        if (dbComment.User.Id != userOfLike.Id)
        {
            var notification = new CommentLikeNotification(dbComment.User, userOfLike.Id, dbComment.UserPost, dbComment);
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }
        
        return Ok(like);
    }
    [HttpPost("AddReply")]
    public async Task<ActionResult<Like>> AddReplyLike(ReplyLikeRequest request)
    {
        var dbReplyRequest = _context.Replies.Where(r => r.Id == request.replyId)
            .Include(r => r.User)
            .Include(r => r.Comment).ThenInclude(c => c.UserPost)
            .Include(r => r.Likes)
            .ThenInclude(l => l.User);;
        if (!dbReplyRequest.Any()) return BadRequest("Reply Not found");
        var dbReply = dbReplyRequest.First();
        //It will be changed to AuthUser
        var userOfLike = await _context.Users.FindAsync(request.userId);
        if (userOfLike == null) return BadRequest("User not Found");
        
        var newReplyLike = new ReplyLike(userOfLike, dbReply);
        
        _context.Likes.Add(newReplyLike);
        await _context.SaveChangesAsync();
        var like = _context.Likes.Where(l => l.User.Id == request.userId && l.Reply.Id == request.replyId)
                .Select(like => new ReplyLikeViewModel(like))
                .First();
        if (dbReply.User.Id != request.userId)
        {
            var notification = new ReplyLikeNotification(dbReply.User, request.userId, dbReply.Comment.UserPost,dbReply.Comment, dbReply );
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }
        return Ok(like);
    }

    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<Like>> Delete(int id)
    {
        var dbLike =  _context.Likes.Find(id);
        if (dbLike == null) return BadRequest("Like not Found");
        _context.Likes.Remove(dbLike);
        await _context.SaveChangesAsync();
        return Ok(dbLike.Id);
    }
    
}