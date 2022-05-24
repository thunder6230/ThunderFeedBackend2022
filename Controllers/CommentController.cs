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
    [HttpGet("Comment/Get/{id}")]
    public async Task<ActionResult<Comment>> Get(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        return Ok(comment);
    }
    [HttpPost("Comment/Post/Add")]
    public async Task<ActionResult<Comment>> AddPostComment([FromBody] AddCommentModel request)
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

    [HttpDelete("/Comment/Delete/{id}")]

    public async Task<ActionResult<Comment>> DeleteComment(int id)
    {
        var dbComment = await _context.Comments
            .Where(c => c.Id == id)
            .Include(c => c.Likes)
            .FirstAsync();
        _context.Comments.Remove(dbComment);
        await _context.SaveChangesAsync();
        return Ok(dbComment.Id);
    }
    [HttpPut("Comment/Update")]
    public async Task<ActionResult<Comment>> UpdatePost([FromBody] UpdateCommentRequest request)
    {
        var dbComment = await _context.Comments.Where(c => c.Id == request.CommentId).Include(c => c.User).FirstOrDefaultAsync();
        if (dbComment == null) return BadRequest("Comment Not Found");
        if (dbComment.User.Id != request.UserId) return BadRequest("You have no Permission to edit this Comment");
        dbComment.UpdatedAt = DateTime.Now;
        dbComment.Body = request.Body;
        await _context.SaveChangesAsync();
        return Ok(dbComment.Body);
    }
    
    

}