using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationController : Controller
{
    private readonly ApplicationDbContext _context;

    // DB Data Contect
    public NotificationController(ApplicationDbContext context)
    {
        _context = context;
    }
    // GET
    [Authorize]
    [HttpGet("GetAll/{userId}")]
    public ActionResult<Notification> GetAll(int userId)
    {
        var userNotifications = _context.Notifications
            .Where(n => n.User.Id == userId)
            .Include(n => n.Comment)
            .Include(n => n.UserPost)
            .Select(n => new NotificationViewModel()
            {
                Id = n.Id,
                CommentId = n.Comment.Id,
                ReplyId = n.Reply.Id,
                IsUnread = n.IsUnread,
                Type = n.Type,
                UserFrom = new UserViewModel(n.UserFromId, n.u),
                UserPostId = n.UserPost.Id
            }).AsSplitQuery().ToList();
      
        return Ok(userNotifications);
    }
    
    [Authorize]
    [HttpGet("GetAllUnread/{userId}")]
    public ActionResult<Notification> GetAllUnread(int userId)
    {
        var unreadNotifications = _context.Notifications
            .Where(n => n.User.Id == userId)
            .Select(n => new NotificationViewModel()
            {
                Id = n.Id,
                CommentId = n.Comment.Id,
                ReplyId = n.Reply.Id,
                IsUnread = n.IsUnread,
                Type = n.Type,
                UserFrom = new UserViewModel(n.UserFromId, _context),
                UserId = n.User.Id,
                UserPostId = n.UserPost.Id
            }).ToList();

        return Ok(unreadNotifications);
    }
    
    [Authorize]
    [HttpPost("MarkRead")]
    public ActionResult MarkAsRead(MarkReadNotificationRequest request)
    {
        var notification = _context.Notifications.First(n => n.Id == request.NotificationId);
        if (notification.User.Id != request.UserId) return Unauthorized("It's not your Notification");

        notification.IsUnread = false;
        _context.SaveChanges();
        return Ok();
      
    }
    [HttpDelete("Delete/{id}")]
    public ActionResult DeleteNotification(int id)
    {
        var notification = _context.Notifications.First(n => n.Id == id);
        // if (dbLike == null) return BadRequest("Like not Found");
        _context.Notifications.Remove(notification);
        _context.SaveChangesAsync();
        return Ok(notification.Id);
    }
}