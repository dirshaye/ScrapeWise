/// <summary>
/// Represents a user profile with display and scraping preferences.
/// </summary>
public class Profile
{
    /// <summary>
    /// Primary key for the profile.
    /// </summary>
    public int ProfileId { get; set; }
    /// <summary>
    /// Display name for the user.
    /// </summary>
    public string DisplayName { get; set; } = "Default User";
    /// <summary>
    /// Avatar image URL.
    /// </summary>
    public string AvatarUrl { get; set; } = "https://www.gravatar.com/avatar/?d=mp";
    /// <summary>
    /// Custom user-agent string for scraping.
    /// </summary>
    public string? UserAgent { get; set; } = string.Empty;
    /// <summary>
    /// Delay between requests in milliseconds.
    /// </summary>
    public int DelayBetweenRequests { get; set; }
    /// <summary>
    /// Foreign key to the user.
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    /// <summary>
    /// Navigation property to the user.
    /// </summary>
    public MyUser? User { get; set; }
} 