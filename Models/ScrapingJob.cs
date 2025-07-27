using System;
using System.Collections.Generic;

/// <summary>
/// Represents a scraping job with details, results, and tags (M-N relationship).
/// </summary>
public class ScrapingJob
{
    /// <summary>
    /// Primary key for the scraping job.
    /// </summary>
    public int ScrapingJobId { get; set; }
    /// <summary>
    /// The target URL to scrape.
    /// </summary>
    public string TargetUrl { get; set; } = string.Empty;
    /// <summary>
    /// The CSS selector used for scraping.
    /// </summary>
    public string CssSelector { get; set; } = string.Empty;
    /// <summary>
    /// The UTC date and time when the job was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Foreign key to the user who owns the job.
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    /// <summary>
    /// Navigation property to the user.
    /// </summary>
    public MyUser? User { get; set; }
    /// <summary>
    /// Navigation property for the results of this job (1-N relationship).
    /// </summary>
    public ICollection<ScrapingResult> ScrapingResults { get; set; } = new List<ScrapingResult>();
    /// <summary>
    /// Many-to-many relationship with tags.
    /// </summary>
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
