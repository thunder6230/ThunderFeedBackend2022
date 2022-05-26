namespace Backend.Models;

public class Picture
{
    public int Id { get; set; }
    public User User { get; set; }
    public string ImgPath { get; set; }
    public string FileName { get; set; }
    
    public UserPost? UserPost { get; set; }
}