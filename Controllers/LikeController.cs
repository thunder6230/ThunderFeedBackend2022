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
        var like = await _context.Likes.Where(l => l.User.Id == request.userId && l.UserPost.Id == request.postId)
            .FirstOrDefaultAsync();
        return Ok(like);
        // try
        // {
        //
        // }
        // catch
        // {
        //     return BadRequest("Error at adding like to Post");
        // }
    }
    [HttpPost("AddComment")]
    public async Task<ActionResult<UserPost>> AddCommentLike(int id)
    {
        var dbComment = await _context.Comments.FindAsync(id);
        if (dbComment == null) return BadRequest("Comment Not found");
        
        //It will be changed to AuthUser
        var userOfLike = await _context.Users.FindAsync(4);
        if (userOfLike == null) return BadRequest("User not Found");
        try
        {
            Like newCommentLike = new Like();
            newCommentLike.User = userOfLike;
            newCommentLike.Comment = dbComment;
            newCommentLike.CreatedAt = DateTime.Now;
            _context.Likes.Add(newCommentLike);
            return Ok(new
            {
                newCommentLike.Id, newCommentLike.CreatedAt, 
            });

        }
        
        catch
        {
            return BadRequest("Error at adding like to Post");
        }
    }

    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<Like>> DeletePost(int id)
    {
        var dbLike = await _context.Likes.FindAsync(id);
        if (dbLike == null) return BadRequest("Like not Found");
        _context.Likes.Remove(dbLike);
        await _context.SaveChangesAsync();
        return Ok(dbLike.Id);
        /*try
        {
        }
        catch
        {
            return BadRequest("Like could not be deleted");
        }*/
    }
}