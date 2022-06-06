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
        var dbUserPost = await _context.UserPosts.FindAsync(request.postId);
        if (dbUserPost == null) return BadRequest("Post Not found");
        
        //It will be changed to AuthUser
        var userOfLike = await _context.Users.FindAsync(request.userId);
        if (userOfLike == null) return BadRequest("User not Found");

        var existingLike = await _context.Likes.Where(l => l.User.Id == request.userId && l.UserPost.Id == request.postId)
            .FirstOrDefaultAsync();
        if (existingLike != null) return BadRequest("You have already Liked this Post");
        Like newPostLike = new Like();
        newPostLike.User = userOfLike;
        newPostLike.UserPost = dbUserPost;
        newPostLike.CreatedAt = DateTime.Now; 
        // newPostLike.
        _context.Likes.Add(newPostLike);
        await _context.SaveChangesAsync();
        var like = _context.Likes.Where(l => l.User.Id == request.userId && l.UserPost.Id == request.postId).Select(l => new PostLikeViewModel()
        {
            Id = l.Id,
            UserId = l.User.Id,
            UserPostId = l.UserPost.Id
        })
            .First();
        return Ok(like);
      
    }
    [HttpPost("AddComment")]
    public async Task<ActionResult<Like>> AddCommentLike(CommentLikeRequest request)
    {
        var dbComment = _context.Comments.First(c => c.Id == request.commentId);
        if (dbComment == null) return BadRequest("Comment Not found");
        
        var userOfLike = _context.Users.First(u => u.Id == request.userId);
        if (userOfLike == null) return BadRequest("User not Found");
        
        Like newCommentLike = new Like();
        newCommentLike.User = userOfLike;
        newCommentLike.Comment = dbComment;
        newCommentLike.CreatedAt = DateTime.Now;
        _context.Likes.Add(newCommentLike);
        await _context.SaveChangesAsync();
        var like = _context.Likes.Where(l => l.User.Id == request.userId && l.Comment.Id == request.commentId).Select(l => new CommentLikeViewModel()
            {
                Id = l.Id,
                UserId = l.User.Id,
                CommentId = l.Comment.Id
            })
            .First();
        return Ok(like);
    }
    [HttpPost("AddReply")]
    public async Task<ActionResult<Like>> AddReplyLike(ReplyLikeRequest request)
    {
        var dbReply = await _context.Replies.FindAsync(request.replyId);
        if (dbReply == null) return BadRequest("Reply Not found");
        
        //It will be changed to AuthUser
        var userOfLike = await _context.Users.FindAsync(request.userId);
        if (userOfLike == null) return BadRequest("User not Found");
        Like newReplyLike = new Like();
        newReplyLike.User = userOfLike;
        newReplyLike.Reply = dbReply;
        newReplyLike.CreatedAt = DateTime.Now;
        _context.Likes.Add(newReplyLike);
        await _context.SaveChangesAsync();
        var like = _context.Likes.Where(l => l.User.Id == request.userId && l.Reply.Id == request.replyId)
                .Select(l => new PostLikeViewModel()
                {
                    Id = l.Id,
                    UserId = l.User.Id,
                    UserPostId = l.UserPost.Id
                })
                .First();
        return Ok(like);
    }

    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<Like>> DeletePost(int id)
    {
        var dbLike = await _context.Likes.FindAsync(id);
        if (dbLike == null) return BadRequest("Like not Found");
        _context.Likes.Remove(dbLike);
        await _context.SaveChangesAsync();
        return Ok(dbLike.Id);
    }
}