using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class CommentController : Controller
{
    private readonly ApplicationDbContext _context;

    // DB Data Contect
    public CommentController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<List<Comment>>> Get()
    {
        var posts = await _context.Comments.ToListAsync();
        return Ok(posts);
    }
    [HttpPost("/Post/Comment/Add")]
    public async Task<ActionResult<Comment>> AddPostComment(AddCommentModel request)
    {
        var post = await _context.UserPosts.FindAsync(request.PostId);
        if (post == null) return BadRequest("Post not Found");
        //It will be changed to AuthUser
        var userOfComment = await _context.Users.FindAsync(request.UserId);
        if (userOfComment == null) return BadRequest("User not Found");
        try
        {
            var comment = new Comment();
            comment.Body = request.Body;
            comment.User = userOfComment;
            comment.CreatedAt = DateTime.Now;
            comment.UserPost = post;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            var newComment = _context.Comments
                .Where(c => c.User.Id == request.UserId && c.UserPost.Id == request.PostId )
                .Include(C => C.Likes);
            return Ok(newComment);
        }
        catch
        {
            return BadRequest("Comment could not be created");
        }
    }
}