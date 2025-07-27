using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System;

/// <summary>
/// Controller responsible for managing scraping results
/// Handles operations related to ScrapingResult entities only
/// </summary>
[Authorize]
public class ResultsController : Controller
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the ResultsController
    /// </summary>
    /// <param name="context">Database context for data operations</param>
    public ResultsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets the current authenticated user from the database
    /// Uses claims-based authentication to retrieve user email
    /// </summary>
    /// <returns>Current user or null if not found</returns>
    private async Task<MyUser> GetCurrentUserAsync()
    {
        var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
    }

    /// <summary>
    /// Displays results for a specific scraping job with pagination
    /// Implements pagination algorithm for large result sets
    /// </summary>
    /// <param name="jobId">ID of the scraping job</param>
    /// <param name="page">Current page number (1-based)</param>
    /// <param name="pageSize">Number of results per page</param>
    /// <returns>View with paginated results</returns>
    public async Task<IActionResult> Index(int jobId, int page = 1, int pageSize = 50)
    {
        var job = await _context.ScrapingJobs
            .Include(j => j.User)
            .FirstOrDefaultAsync(j => j.ScrapingJobId == jobId);

        if (job == null) return NotFound();

        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null) return Unauthorized();

        // Authorization algorithm - role-based access control
        if (!User.IsInRole("Admin") && job.UserId != currentUser.Id) 
            return Forbid();

        // Pagination algorithm - calculate total count and offset
        var totalResults = await _context.ScrapingResults
            .Where(r => r.ScrapingJobId == jobId)
            .CountAsync();

        var results = await _context.ScrapingResults
            .Where(r => r.ScrapingJobId == jobId)
            .OrderByDescending(r => r.ScrapedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Pagination metadata calculation
        ViewBag.Job = job;
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalResults = totalResults;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalResults / pageSize);
        ViewBag.HasPreviousPage = page > 1;
        ViewBag.HasNextPage = page < ViewBag.TotalPages;

        return View(results);
    }

    /// <summary>
    /// Exports scraping job results as CSV file
    /// Implements CSV generation algorithm with proper escaping
    /// </summary>
    /// <param name="jobId">ID of the scraping job to export</param>
    /// <returns>CSV file download</returns>
    [HttpGet]
    public async Task<IActionResult> ExportCsv(int jobId)
    {
        var job = await _context.ScrapingJobs
            .Include(j => j.ScrapingResults)
            .Include(j => j.User)
            .FirstOrDefaultAsync(j => j.ScrapingJobId == jobId);

        if (job == null) return NotFound();

        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null) return Unauthorized();

        // Authorization algorithm - only owners and admins can export
        if (!User.IsInRole("Admin") && job.UserId != currentUser.Id) 
            return Forbid();

        // CSV generation algorithm with proper escaping
        var csv = new StringBuilder();
        csv.AppendLine("\"ExtractedText\",\"ScrapedAt\",\"JobUrl\",\"CssSelector\",\"JobId\"");

        foreach (var result in job.ScrapingResults)
        {
            // Text escaping algorithm - handle CSV special characters
            var escapedText = EscapeCsvField(result.ExtractedText);
            var escapedUrl = EscapeCsvField(job.TargetUrl);
            var escapedSelector = EscapeCsvField(job.CssSelector);
            
            csv.AppendLine($"\"{escapedText}\",\"{result.ScrapedAt:O}\",\"{escapedUrl}\",\"{escapedSelector}\",{job.ScrapingJobId}");
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        var fileName = $"scraping_job_{jobId}_results_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
        
        return File(bytes, "text/csv", fileName);
    }

    /// <summary>
    /// Escapes CSV field content to handle special characters
    /// Implements RFC 4180 CSV escaping algorithm
    /// </summary>
    /// <param name="field">Field content to escape</param>
    /// <returns>Escaped field content safe for CSV</returns>
    private string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
            return string.Empty;

        // RFC 4180 CSV escaping algorithm
        return field
            .Replace("\"", "\"\"")  // Escape quotes by doubling them
            .Replace("\n", " ")     // Replace newlines with spaces
            .Replace("\r", " ")     // Replace carriage returns with spaces
            .Replace("\t", " ");    // Replace tabs with spaces
    }

    /// <summary>
    /// Exports results in JSON format for API consumption
    /// Implements JSON serialization with metadata
    /// </summary>
    /// <param name="jobId">ID of the scraping job to export</param>
    /// <returns>JSON file download</returns>
    [HttpGet]
    public async Task<IActionResult> ExportJson(int jobId)
    {
        var job = await _context.ScrapingJobs
            .Include(j => j.ScrapingResults)
            .Include(j => j.Tags)
            .Include(j => j.User)
            .FirstOrDefaultAsync(j => j.ScrapingJobId == jobId);

        if (job == null) return NotFound();

        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null) return Unauthorized();

        // Authorization algorithm
        if (!User.IsInRole("Admin") && job.UserId != currentUser.Id) 
            return Forbid();

        // JSON structure algorithm - create hierarchical data
        var exportData = new
        {
            Job = new
            {
                job.ScrapingJobId,
                job.TargetUrl,
                job.CssSelector,
                job.CreatedAt,
                Owner = job.User?.UserName,
                Tags = job.Tags.Select(t => new { t.TagId, t.Name, t.Color })
            },
            Results = job.ScrapingResults.Select(r => new
            {
                r.ExtractedText,
                r.ScrapedAt
            }),
            Metadata = new
            {
                ExportedAt = DateTime.UtcNow,
                TotalResults = job.ScrapingResults.Count,
                ExportedBy = currentUser.UserName
            }
        };

        var json = System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });

        var bytes = Encoding.UTF8.GetBytes(json);
        var fileName = $"scraping_job_{jobId}_results_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
        
        return File(bytes, "application/json", fileName);
    }

    /// <summary>
    /// Deletes a specific scraping result
    /// Implements single result deletion with authorization
    /// </summary>
    /// <param name="id">Result ID to delete</param>
    /// <returns>Redirect to results index</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _context.ScrapingResults
            .Include(r => r.ScrapingJob)
            .FirstOrDefaultAsync(r => r.ScrapingResultId == id);

        if (result == null) return NotFound();

        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null) return Unauthorized();

        // Authorization algorithm - check job ownership
        if (!User.IsInRole("Admin") && result.ScrapingJob.UserId != currentUser.Id) 
            return Forbid();

        var jobId = result.ScrapingJobId;
        _context.ScrapingResults.Remove(result);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Result deleted successfully.";
        return RedirectToAction("Index", new { jobId });
    }
}
