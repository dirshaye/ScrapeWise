using System.Collections.Generic;

public class Tag
{
    public int TagId { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// color of the tag, used for UI representation<br />
    /// Default is a purple color.<br />
    /// Can be overridden by the user.<br />
    /// The color is represented in hexadecimal format.<br />
    /// Example: "#6a11cb"
    /// </summary>
    public string Color { get; set; } = "#6a11cb"; // Default color

    /// <summary>
    /// many to many relationship with ScrapingJob
    /// </summary>
    public ICollection<ScrapingJob> ScrapingJobs { get; set; } = new List<ScrapingJob>();
} 