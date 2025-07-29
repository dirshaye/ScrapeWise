using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Application user entity, extends IdentityUser for authentication and authorization.
/// </summary>
public class MyUser : IdentityUser
{
    /// <summary>
    /// Indicates if the user is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
    /// <summary>
    /// The UTC date and time when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Navigation property to the user's profile.
    /// </summary>
    public Profile? Profile { get; set; }
    /// <summary>
    /// Navigation property for the user's scraping jobs (M-N join).
    /// </summary>
    public ICollection<ScrapingJob> ScrapingJobs { get; set; } = new List<ScrapingJob>();
}
