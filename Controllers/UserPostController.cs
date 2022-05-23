using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace Backend.Controllers;
[Authorize]
[ApiController]
[Route("[controller]")]
public class UserPostController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    // GET
    public UserPostController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("getAll")]
    [AllowAnonymous]
    public async Task<ActionResult<ICollection<UserPost>>> Get()
    {
        try
        {
            ICollection<UserPost> posts = await _context.UserPosts
                .Include(post => post.User)
                .Include(post => post.Comments)
                .ThenInclude(c => c.Likes)
                .Include(post => post.Likes)
                .OrderByDescending(x => x.CreatedAt).ToListAsync();
            if (posts.Count == 0) return BadRequest("There are no Posts yet");
            return Ok(posts);
        }
        catch
        {
            return BadRequest("Please Log in to see the Content");
        }
    }

    [HttpGet("getPost/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserPost>> Get(int id)
    {
        var userPost = await _context.UserPosts.Where(p => p.Id == id)
            .Include(post => post.Likes)
            .Include(post => post.User)
            .Include(post => post.Comments)
            .FirstOrDefaultAsync();
        if (userPost == null) return BadRequest("Post Not Found");
        return Ok(userPost);
    }

    [HttpPost("AddPost")]
    public async Task<ActionResult<UserPost>> Post([FromBody] AddPostModel request)
    {
        var userPost = new UserPost();
        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null) return BadRequest("User Not Found");
        userPost.User = user;
        userPost.Body = request.Body;
        userPost.CreatedAt = DateTime.Now;
        try
        {
            _context.UserPosts.Add(userPost);
            await _context.SaveChangesAsync();

            /*var newPost = user.UserPosts.First();
            newPost.User.UserPosts = new List<UserPost>();
            newPost.User.PasswordHash = null;*/
            var newPost = await _context.UserPosts.OrderByDescending(p => p.CreatedAt)
                .Where(p => p.User.Id == request.UserId)
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .FirstOrDefaultAsync();

            return Ok(newPost);

        }
        catch
        {
            return BadRequest("Something went wrong please try again");
        }
    }

    [HttpPut("UpdatePost")]
    public async Task<ActionResult<UserPost>> UpdatePost([FromBody] UpdatePostRequest request)
    {
        var dbComment = await _context.UserPosts.Where(p => p.Id == request.PostId).Include(p => p.User).FirstOrDefaultAsync();
        if (dbComment == null) return BadRequest("Comment Not Found");
        if (dbComment.User.Id != request.UserId) return BadRequest("You have no Permission to edit this Post");
        dbComment.UpdatedAt = DateTime.Now;
        dbComment.Body = request.Body;
        await _context.SaveChangesAsync();
        return Ok(dbComment.Body);
    }
    
    [HttpDelete("DeletePost/{id}")]
    public async Task<ActionResult<UserPost>> Delete(int id)
    {
        var dbUserPost = await _context.UserPosts
            .Where(p => p.Id == id)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .FirstAsync();
        if (dbUserPost == null) return BadRequest("Post Not Found");
        _context.UserPosts.Remove(dbUserPost);
        await _context.SaveChangesAsync();
        return Ok(dbUserPost.Id);
    }

}