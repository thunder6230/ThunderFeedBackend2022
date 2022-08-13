using System.IdentityModel.Tokens.Jwt;
using System.Security.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;


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
    public ActionResult<ICollection<UserPost>> GetAll(int count = 5, int page = 1)
    {
        var posts = _context.UserPosts.OrderByDescending(x => x.CreatedAt)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .Include(p => p.Pictures)
            .Include(p => p.User)
            .Take(count).Skip(count * (page - 1)).ToList();

        var viewPosts = GetViewPosts(posts);

        return Ok(viewPosts);
    }

    [HttpGet("getUserPosts/{id}")]
    [AllowAnonymous]
    public ActionResult<ICollection<UserPost>> GetAllUser(int id)
    {

        var posts = _context.UserPosts.Where(p => p.User.Id == id || p.UserToId == id)
            .OrderByDescending(p => p.CreatedAt).Take(20).ToList();
            var viewPosts = GetViewPosts(posts);

            return Ok(viewPosts);
        
    }


    [HttpGet("getPost/{id}")]
    [AllowAnonymous]
    public ActionResult<UserPost> Get(int id)
    {
        var post = _context.UserPosts.Where(p => p.Id == id).First();
        var viewPost = new UserPostViewModel(post, _context);

        return Ok(viewPost);
    }

   
    [HttpPost("AddPost")]
    public async Task<ActionResult<UserPost>> Post(IFormCollection formCollection)
    {
        
        if (formCollection.Files.Count != 0)
        {
            var sizeErrors = ValidateFileSize(formCollection.Files);
            if (sizeErrors.Count > 0) return BadRequest(sizeErrors);
            var extensionErrors = ValidateFileExtension(formCollection.Files);
            if (extensionErrors.Count > 0) return BadRequest(extensionErrors);

        }

        var userId = int.Parse(formCollection["UserId"]);
        bool hasUserTo = formCollection.ContainsKey("UserToId");
        int userToId = -1;
        User userTo = new User();
        if(hasUserTo) userToId = int.Parse(formCollection["UserToId"]);
        var body = formCollection["Body"];

        var user = _context.Users.First(u => u.Id == userId);
        if (user == null) return BadRequest("User Not Found");
        if(hasUserTo && userToId > -1) userTo = _context.Users.First(u => u.Id == userToId);
        // if (userTo == null) return BadRequest("User Not Found");

        var userPost = new UserPost();
        userPost.User = user;
        if(hasUserTo) userPost.UserToId = userToId;
        userPost.Body = body;
        userPost.CreatedAt = DateTime.Now;

        _context.UserPosts.Add(userPost);
        await _context.SaveChangesAsync();
        var newPost = _context.UserPosts.Where(p => p.User.Id == userId).OrderByDescending(p => p.CreatedAt).First();


        if (formCollection.Files.Count > 0) await SaveImages(formCollection.Files, user, newPost);

        var post = new UserPostViewModel(newPost, _context);
        
       
        if (hasUserTo)
        {
            var notification = new PostToUserNotification( userTo,user.Id, newPost);
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }
        if (newPost == null) return BadRequest("Post could not be created");

        return Ok(post);
    }

    [HttpPut("UpdatePost")]
    public async Task<ActionResult<UserPost>> UpdatePost([FromBody] UpdatePostRequest request)
    {
        var dbComment = await _context.UserPosts.Where(p => p.Id == request.PostId).Include(p => p.User)
            .FirstOrDefaultAsync();
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
            .Include(p => p.Pictures)
            .FirstAsync();
        if (dbUserPost == null) return BadRequest("Post Not Found");
        _context.UserPosts.Remove(dbUserPost);
        await _context.SaveChangesAsync();
        return Ok(dbUserPost.Id);
    }

    private List<ValidationError> ValidateFileExtension(IFormFileCollection fileCollection)
    {
        var supportedTypes = new[] { "jpg", "bmp", "png", "jpeg" };
        List<ValidationError> ValidationErrors = new List<ValidationError>();
        foreach (var file in fileCollection)
        {
            var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1).ToLower();
            var fileName = file.FileName;
            var errorMessage = "File Extension Is InValid - Only Upload jpg/jpeg/bmp/png File";
            if (!supportedTypes.Contains(fileExt))
            {
                var validationError = new ValidationError();
                validationError.FileName = fileName;
                validationError.ErrorMessage = errorMessage;
                ValidationErrors.Add(validationError);
            }
        }

        return ValidationErrors;
    }

    private List<ValidationError> ValidateFileSize(IFormFileCollection fileCollection)
    {
        var maxAllowedSize = 2621440;
        List<ValidationError> ValidationErrors = new List<ValidationError>();
        foreach (var file in fileCollection)
        {
            var fileName = file.FileName;
            var errorMessage = "File Size Is InValid - Max Upload Size is 2,5mb";
            if (file.Length > maxAllowedSize)
            {
                var validationError = new ValidationError();
                validationError.FileName = fileName;
                validationError.ErrorMessage = errorMessage;
                ValidationErrors.Add(validationError);
            }
        }

        return ValidationErrors;
    }

    private async Task<List<Picture>> SaveImages(IFormFileCollection fileCollection, User user, UserPost post)
    {
        List<Picture> pictures = new List<Picture>();
        foreach (var file in fileCollection)
        {
            string uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            string fileName = file.FileName;
            string extension = Path.GetExtension(fileName);
            string hashedFilename = Guid.NewGuid() + extension;
            string imagePath = Path.Combine(uploadDirectory, hashedFilename);
            await using var stream = new FileStream(imagePath, FileMode.Create);

            await file.CopyToAsync(stream);
            stream.Flush();

            string url = "Uploads/" + hashedFilename;
            Picture picture = new Picture();
            picture.User = user;
            picture.FileName = hashedFilename;
            picture.ImgPath = url;
            picture.UserPost = post;
            pictures.Add(picture);
            _context.Pictures.Add(picture);


        }

        await _context.SaveChangesAsync();
        var dbPictures = await _context.Pictures.Where(p => p.UserPost.Id == post.Id).ToListAsync();
        return dbPictures;
    }

    private List<UserPostViewModel> GetViewPosts(List<UserPost> posts)
    {
        var viewPosts = new List<UserPostViewModel>();
        foreach (var post in posts)
        {
            viewPosts.Add(new UserPostViewModel(post, _context));
        }

        return viewPosts;
    }

}