using System;
using System.Collections.Generic;

public class ScrapingJob
{
    public int ScrapingJobId { get; set; }
    public string TargetUrl { get; set; }
    public string CssSelector { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int UserId { get; set; }
    public User User { get; set; }

    public ICollection<ScrapingResult> ScrapingResults { get; set; }
}
