using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

/// <summary>
/// API controller for managing scraping jobs and their relationships.
/// </summary>
[Route("api/jobs")]
[ApiController]
[Authorize]
public class JobsApiController : ControllerBase
{
    private readonly AppDbContext _context;
    public JobsApiController(AppDbContext context) { _context = context; }

    /// <summary>
    /// Gets all jobs with their tags and results (M-N and 1-N relationships).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetJobs()
    {
        var jobs = await _context.ScrapingJobs
            .Include(j => j.Tags)
            .Include(j => j.ScrapingResults)
            .Include(j => j.User)
            .Select(j => new {
                j.ScrapingJobId,
                j.TargetUrl,
                j.CssSelector,
                j.CreatedAt,
                User = j.User.UserName,
                Tags = j.Tags.Select(t => new { t.TagId, t.Name }),
                Results = j.ScrapingResults.Select(r => new { r.ExtractedText, r.ScrapedAt })
            })
            .ToListAsync();
        return Ok(jobs);
    }
} 