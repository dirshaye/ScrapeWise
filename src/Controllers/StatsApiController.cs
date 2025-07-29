using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

/// <summary>
/// API controller for application statistics.
/// </summary>
[Route("api/stats")]
[ApiController]
[Authorize]
public class StatsApiController : ControllerBase
{
    private readonly AppDbContext _context;
    public StatsApiController(AppDbContext context) { _context = context; }

    /// <summary>
    /// Gets statistics for jobs, results, tags, and users.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        var stats = new
        {
            TotalJobs = await _context.ScrapingJobs.CountAsync(),
            TotalTags = await _context.Tags.CountAsync(),
            TotalUsers = await _context.Users.CountAsync()
        };
        return Ok(stats);
    }
} 