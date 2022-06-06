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
    public async Task<ActionResult<Comment>> AddPostComment(IFormCollection formCollection)
    {
        if (formCollection.Files.Count != 0)
        {
            var sizeErrors = ValidateFileSize(formCollection.Files);
            if (sizeErrors.Count > 0) return BadRequest(sizeErrors);
            var extensionErrors = ValidateFileExtension(formCollection.Files);
            if (extensionErrors.Count > 0) return BadRequest(extensionErrors);
            
        }

        var userId = int.Parse(formCollection["UserId"]);
        var postId = int.Parse(formCollection["PostId"]);
        var body = formCollection["Body"];
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return BadRequest("User Not Found");
        var post = await _context.UserPosts.FindAsync(postId);
        if (post == null) return BadRequest("Post not Found");
        //It will be changed to AuthUser
        var userOfComment = await _context.Users.FindAsync(userId);
        if (userOfComment == null) return BadRequest("User not Found");
        
            var comment = new Comment();
            comment.Body = body;
            comment.User = userOfComment;
            comment.CreatedAt = DateTime.Now;
            comment.UserPost = post;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            
            
            var newComment = _context.Comments.Where(c => c.User.Id == userId).First();

            if (formCollection.Files.Count > 0) await SaveImages(formCollection.Files, user, newComment);

            var newCommentOptimised = _context.Comments.Where(c => c.Id == newComment.Id)
                .Select(c => new CommentViewModel()
                {
                    Id = c.Id,
                    Body = c.Body,
                    CreatedAt = c.CreatedAt,
                    UserPostId = c.UserPost.Id,
                    Pictures = c.Pictures.Select(pic => new PictureViewModel()),
                    User = new UserViewModel()
                    {
                        Id = c.User.Id, Email = c.User.Email, FirstName = c.User.FirstName, LastName = c.User.LastName
                    },
                    Likes = c.Likes.Select(l => new CommentLikeViewModel())
                }).First();

            return Ok(newCommentOptimised);
        
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
        var dbComment = await _context.Comments.Where(c => c.Id == request.CommentId).Include(c => c.User)
            .FirstOrDefaultAsync();
        if (dbComment == null) return BadRequest("Comment Not Found");
        if (dbComment.User.Id != request.UserId) return BadRequest("You have no Permission to edit this Comment");
        dbComment.UpdatedAt = DateTime.Now;
        dbComment.Body = request.Body;
        await _context.SaveChangesAsync();
        return Ok(dbComment.Body);
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

    private async Task<List<Picture>> SaveImages(IFormFileCollection fileCollection, User user, Comment comment)
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
            picture.Comment = comment;
            pictures.Add(picture);
            _context.Pictures.Add(picture);
            
        }

        await _context.SaveChangesAsync();
        var dbPictures = await _context.Pictures.Where(p => p.Comment.Id == comment.Id).ToListAsync();
        return dbPictures;
    }
}