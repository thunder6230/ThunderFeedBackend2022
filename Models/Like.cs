using Microsoft.Extensions.Options;

namespace Backend.Models;

public class Like
{
    public int Id { get; set; }
    public User User { get; set; }
    public UserPost? UserPost { get; set; }
    public Comment? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}