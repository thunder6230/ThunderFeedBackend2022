using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class PictureController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    // GET
    public PictureController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Picture>> getPicture(int id)
    {
        var picture = await _context.Pictures.FindAsync(id);
        if (picture == null) return BadRequest("Picture Not Found");
        var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + picture.ImgPath);
        imgPath = imgPath.Replace("/", "\\");
        Byte[] b = System.IO.File.ReadAllBytes(imgPath);   // You can use your own method over here.         
        return File(b, "image/jpeg");
    }
}
    