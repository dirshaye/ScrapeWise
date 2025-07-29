/// <summary>
/// View model for displaying tags with usage statistics
/// Used in the Tags Index page to show tag information and job counts
/// </summary>
public class TagViewModel
{
    /// <summary>
    /// The tag entity containing tag information
    /// </summary>
    public Tag Tag { get; set; } = null!;

    /// <summary>
    /// Number of scraping jobs associated with this tag
    /// </summary>
    public int JobCount { get; set; }
}
