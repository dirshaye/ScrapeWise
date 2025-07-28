using Microsoft.EntityFrameworkCore;

/// <summary>
/// Service for Tag-related operations
/// Implements business logic for tag management and ensures separation of concerns
/// </summary>
public interface ITagService
{
    /// <summary>
    /// Gets all available tags for job assignment
    /// </summary>
    /// <returns>List of all tags</returns>
    Task<List<Tag>> GetAllTagsAsync();
    
    /// <summary>
    /// Gets tags by their IDs
    /// </summary>
    /// <param name="tagIds">Array of tag IDs to retrieve</param>
    /// <returns>List of matching tags</returns>
    Task<List<Tag>> GetTagsByIdsAsync(int[] tagIds);
}

/// <summary>
/// Implementation of Tag service
/// Handles all Tag entity operations with proper separation of concerns
/// </summary>
public class TagService : ITagService
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the TagService
    /// </summary>
    /// <param name="context">Database context for tag operations</param>
    public TagService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all available tags for job assignment
    /// Algorithm: Simple EF Core query with no filtering
    /// </summary>
    /// <returns>List of all tags</returns>
    public async Task<List<Tag>> GetAllTagsAsync()
    {
        // Tag retrieval algorithm - get all active tags
        return await _context.Tags.ToListAsync();
    }

    /// <summary>
    /// Gets tags by their IDs for job-tag relationship management
    /// Algorithm: Filtered query using Contains() for IN clause SQL generation
    /// </summary>
    /// <param name="tagIds">Array of tag IDs to retrieve</param>
    /// <returns>List of matching tags</returns>
    public async Task<List<Tag>> GetTagsByIdsAsync(int[] tagIds)
    {
        if (tagIds == null || tagIds.Length == 0)
            return new List<Tag>();

        // Tag filtering algorithm - IN clause equivalent for multiple IDs
        return await _context.Tags
            .Where(t => tagIds.Contains(t.TagId))
            .ToListAsync();
    }
}
