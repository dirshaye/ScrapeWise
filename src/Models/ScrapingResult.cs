using System;

/// <summary>
/// Represents the result of a web scraping operation
/// Contains the extracted text data from a specific scraping job
/// </summary>
public class ScrapingResult
{
    /// <summary>
    /// Primary key for the scraping result
    /// </summary>
    public int ScrapingResultId { get; set; }

    /// <summary>
    /// The text content extracted during the scraping operation
    /// </summary>
    public string ExtractedText { get; set; } = string.Empty;

    /// <summary>
    /// The UTC date and time when this result was scraped
    /// </summary>
    public DateTime ScrapedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Foreign key to the scraping job that produced this result
    /// </summary>
    public int ScrapingJobId { get; set; }

    /// <summary>
    /// Navigation property to the parent scraping job
    /// </summary>
    public ScrapingJob? ScrapingJob { get; set; }
}
