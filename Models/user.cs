using System.Collections.Generic;

public class User
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }

    public ConfigProfile ConfigProfile { get; set; }
    public ICollection<ScrapingJob> ScrapingJobs { get; set; }
}
