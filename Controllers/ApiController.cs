using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ApiController : ControllerBase
{
    private readonly AppDbContext _context;

    public ApiController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/jobs
    [HttpGet("jobs")]
    public async Task<IActionResult> GetJobs()
    {
        var jobs = await _context.ScrapingJobs
            .Include(j => j.ScrapingResults)
            .Include(j => j.Tags)
            .Select(j => new
            {
                j.ScrapingJobId,
                j.TargetUrl,
                j.CssSelector,
                j.CreatedAt,
                ResultsCount = j.ScrapingResults.Count,
                Tags = j.Tags.Select(t => new { t.TagId, t.Name, t.Color })
            })
            .ToListAsync();

        return Ok(jobs);
    }

    // GET: api/jobs/{id}
    [HttpGet("jobs/{id}")]
    public async Task<IActionResult> GetJob(int id)
    {
        var job = await _context.ScrapingJobs
            .Include(j => j.ScrapingResults)
            .Include(j => j.Tags)
            .FirstOrDefaultAsync(j => j.ScrapingJobId == id);

        if (job == null)
            return NotFound();

        return Ok(new
        {
            job.ScrapingJobId,
            job.TargetUrl,
            job.CssSelector,
            job.CreatedAt,
            Results = job.ScrapingResults.Select(r => new { r.ExtractedText, r.ScrapedAt }),
            Tags = job.Tags.Select(t => new { t.TagId, t.Name, t.Color })
        });
    }

    // GET: api/tags
    [HttpGet("tags")]
    public async Task<IActionResult> GetTags()
    {
        var tags = await _context.Tags
            .Select(t => new { t.TagId, t.Name, t.Color, JobsCount = t.ScrapingJobs.Count })
            .ToListAsync();

        return Ok(tags);
    }

    // POST: api/tags
    [HttpPost("tags")]
    public async Task<IActionResult> CreateTag([FromBody] Tag tag)
    {
        if (string.IsNullOrEmpty(tag.Name))
            return BadRequest("Tag name is required");

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTags), new { id = tag.TagId }, tag);
    }

    // PUT: api/jobs/{id}/tags
    [HttpPut("jobs/{id}/tags")]
    public async Task<IActionResult> AssignTags(int id, [FromBody] int[] tagIds)
    {
        var job = await _context.ScrapingJobs
            .Include(j => j.Tags)
            .FirstOrDefaultAsync(j => j.ScrapingJobId == id);

        if (job == null)
            return NotFound();

        var tags = await _context.Tags
            .Where(t => tagIds.Contains(t.TagId))
            .ToListAsync();

        job.Tags.Clear();
        foreach (var tag in tags)
        {
            job.Tags.Add(tag);
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Tags assigned successfully" });
    }

    // GET: api/stats
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = new
        {
            TotalJobs = await _context.ScrapingJobs.CountAsync(),
            TotalResults = await _context.ScrapingResults.CountAsync(),
            TotalTags = await _context.Tags.CountAsync(),
            TotalUsers = await _context.Users.CountAsync()
        };

        return Ok(stats);
    }
} 