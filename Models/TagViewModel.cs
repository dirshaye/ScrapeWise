namespace ScrapeWise_Intelligent_Web_Scraping_Dashboard_ASP.NET_Core_MVC_.Models;

/// <summary>
/// View model for displaying tags with usage statistics
/// </summary>
public class TagViewModel
{
    /// <summary>
    /// The tag entity
    /// </summary>
    public Tag Tag { get; set; } = null!;

    /// <summary>
    /// Number of jobs that use this tag
    /// </summary>
    public int JobCount { get; set; }
}
