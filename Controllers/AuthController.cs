using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Controllers;

[Route("[Controller]")]
[ApiController]
public class AuthController : Controller
{
    
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(
        IConfiguration configuration,
        ApplicationDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }
    
    [HttpPost("Register")]
    public async Task<ActionResult<User>> Register(RegisterModel request)
    {
        var existingUser = await _context.Users.Where(user => user.Email.ToLower() == request.Email.ToLower()).FirstOrDefaultAsync();
        if (existingUser != null) return BadRequest("This User already exists");
        
        var isPasswordMatch = request.Password == request.Password_Confirm;
        if (!isPasswordMatch) return BadRequest("The entered passwords are not identical");
        
        User user = new User();
        CreatePasswordHash(request.Password, out byte[] passwordHash);
        user.Email = request.Email;
        user.PasswordHash = passwordHash;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.CreatedAt = DateTime.Now;
        user.Gender = request.Gender;
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var newUser = await _context.Users.Where(user => user.Email.ToLower() == request.Email.ToLower()).FirstOrDefaultAsync();
        if (newUser == null) return BadRequest("Something went wrong");
        return Ok("User has been successfully created");
    }

    [HttpPost("Login")]
    public async Task<ActionResult<User>> Login(LoginModel request)
    {
        var user = await _context.Users.FirstAsync(user => user.Email.ToLower() == request.Email.ToLower());
        if (user == null) return BadRequest("User Not Found");
        CreatePasswordHash(request.Password, out byte[] passwordHash);
        if (passwordHash.ToString() != user.PasswordHash.ToString()) return BadRequest("Password is incorrect");
        string token = CreateToken(user, request.RememberMe);
        return Ok(token);
    }

    private string CreateToken(User user, bool rememberMe)
    {
        List<Claim> claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Name, user.Email));
        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        var token = GetToken(claims, rememberMe);
        string jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
    private void CreatePasswordHash(string password, out byte[] passwordHash)
    {
        using var hmac = new HMACSHA512();
        hmac.Key = Encoding.UTF8.GetBytes(_configuration["MyAppSettings:Secrets:Salt"]);
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
    
    private JwtSecurityToken GetToken(List<Claim> authClaims, bool rememberMe)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        var expireDate = rememberMe ? DateTime.Now.AddMonths(6) : DateTime.Now.AddHours(1);
        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: expireDate,
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha512)
        );

        return token;
    }
    
    

    
    
}