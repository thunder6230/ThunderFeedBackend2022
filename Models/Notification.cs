using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Options;

namespace Backend.Models;

public class Notification
{
    public int Id { get; set; }
    public User User { get; set; }
    public User UserFrom { get; set; }
    public string Type { get; set; }
    public UserPost? UserPost { get; set; }
    public Comment? Comment { get; set; }
    public Reply? Reply { get; set; }
    public DateTime? ReadAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
}

