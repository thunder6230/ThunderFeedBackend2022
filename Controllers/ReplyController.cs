using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class ReplyController : Controller
{
    private readonly ApplicationDbContext _context;

    // DB Data Contect
    public ReplyController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("Reply/Get/{id}")]
    public async Task<ActionResult<Reply>> Get(int id)
    {
        var reply = await _context.Replies.FindAsync(id);
        return Ok(reply);
    }
    [HttpPost("Reply/Add")]
    public async Task<ActionResult<Reply>> AddCommentReply([FromBody] AddReplyModel request)
    {
        var comment = await _context.Comments.FindAsync(request.CommentId);
        if (comment == null) return BadRequest("Post not Found");
        //It will be changed to AuthUser
        var userOfReply = await _context.Users.FindAsync(request.UserId);
        if (userOfReply == null) return BadRequest("User not Found");
        
            var reply = new Reply();
            reply.Body = request.Body;
            reply.User = userOfReply;
            reply.CreatedAt = DateTime.Now;
            reply.Comment = comment;
            _context.Replies.Add(reply);
            await _context.SaveChangesAsync();
            var newReply = await _context.Replies.Where(r => r.Comment.Id == request.CommentId && r.User.Id == request.UserId)
                .Include(r => r.User)
                .Include(r => r.Comment)
                .Include(r => r.Likes)
                .OrderByDescending(r => r.CreatedAt).FirstOrDefaultAsync();
            if (newReply.User.Id != userOfReply.Id)
            {
                // var notification = new CommentReplyNotification(newReply.User,userOfReply.Id,  commentnewReply, );
                // _context.Notifications.Add(notification);
                // await _context.SaveChangesAsync();
            }
            return Ok(newReply);
        
    }

    [HttpDelete("Reply/Delete/{id}")]

    public async Task<ActionResult<Reply>> DeleteReply(int id)
    {
        var dbReply = await _context.Replies
            .Where(r => r.Id == id)
            .Include(r => r.Likes)
            .FirstAsync();
        _context.Replies.Remove(dbReply);
        await _context.SaveChangesAsync();
        return Ok(dbReply.Id);
    }
    [HttpPut("Reply/Update")]
    public async Task<ActionResult<Reply>> UpdatePost([FromBody] UpdateReplyRequest request)
    {
        var dbReply = await _context.Replies.Where(r => r.Id == request.ReplyId).Include(r => r.User).FirstOrDefaultAsync();
        if (dbReply == null) return BadRequest("Reply Not Found");
        if (dbReply.User.Id != request.UserId) return BadRequest("You have no Permission to edit this Reply");
        dbReply.UpdatedAt = DateTime.Now;
        dbReply.Body = request.Body;
        await _context.SaveChangesAsync();
        return Ok(dbReply.Body);
    }  
    

}