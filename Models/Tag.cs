using System.Collections.Generic;

public class Tag
{
    public int TagId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#6a11cb"; // Default color

    // Many-to-Many relationship with ScrapingJob
    public ICollection<ScrapingJob> ScrapingJobs { get; set; } = new List<ScrapingJob>();
} 