using System;
using System.Collections.Generic;

public class ScrapingJob
{
    public int ScrapingJobId { get; set; }
    public string TargetUrl { get; set; } = string.Empty;
    public string CssSelector { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public User? User { get; set; }

    public ICollection<ScrapingResult> ScrapingResults { get; set; } = new List<ScrapingResult>();
    
    // Many-to-Many relationship with Tag
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
