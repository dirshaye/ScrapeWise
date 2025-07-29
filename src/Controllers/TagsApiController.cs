using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

/// <summary>
/// API controller for managing tags and their relationships.
/// </summary>
[Route("api/tags")]
[ApiController]
[Authorize]
public class TagsApiController : ControllerBase
{
    private readonly AppDbContext _context;
    public TagsApiController(AppDbContext context) { _context = context; }

    /// <summary>
    /// Gets all tags with their associated jobs (M-N relationship).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTags()
    {
        var tags = await _context.Tags
            .Include(t => t.ScrapingJobs)
            .Select(t => new {
                t.TagId,
                t.Name,
                Jobs = t.ScrapingJobs.Select(j => new { j.UserId })
            })
            .ToListAsync();
        return Ok(tags);
    }
} 