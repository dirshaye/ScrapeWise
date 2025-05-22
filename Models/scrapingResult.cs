using System;

public class ScrapingResult
{
    public int ScrapingResultId { get; set; }
    public string ExtractedText { get; set; }
    public DateTime ScrapedAt { get; set; } = DateTime.Now;

    public int ScrapingJobId { get; set; }
    public ScrapingJob ScrapingJob { get; set; }
}
