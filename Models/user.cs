using System.Collections.Generic;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // In production, this should be hashed
    public string Role { get; set; } = "User"; // Admin, User, Guest
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public Profile? Profile { get; set; }
    public ICollection<ScrapingJob> ScrapingJobs { get; set; } = new List<ScrapingJob>();
}
