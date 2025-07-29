namespace ScrapeWise_Intelligent_Web_Scraping_Dashboard_ASP.NET_Core_MVC_.Models;

/// <summary>
/// View model for displaying error information to users
/// Used by the Error view to show request tracking details
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Unique identifier for the request that caused the error
    /// Used for debugging and error tracking
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Indicates whether the RequestId should be displayed to the user
    /// Returns true if RequestId is not null or empty
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
