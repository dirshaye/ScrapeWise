using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ScrapeWise_Intelligent_Web_Scraping_Dashboard_ASP.NET_Core_MVC_.Models;

namespace ScrapeWise_Intelligent_Web_Scraping_Dashboard_ASP.NET_Core_MVC_.Controllers;

/// <summary>
/// Controller responsible for managing tags and tag-job relationships
/// Handles operations related to Tag entities and M-N relationships with ScrapingJob
/// </summary>
[Authorize(Roles = "Admin")]
public class TagsController : Controller
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the TagsController
    /// </summary>
    /// <param name="context">Database context for data operations</param>
    public TagsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets the current authenticated user from the database
    /// </summary>
    /// <returns>Current user or null if not found</returns>
    private async Task<MyUser?> GetCurrentUserAsync()
    {
        var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
    }

    // GET: /Tags
    /// <summary>
    /// Displays all tags in the system with usage statistics
    /// </summary>
    /// <returns>View with list of tags</returns>
    public IActionResult Index()
    {
        var tags = _context.Tags
            .Select(t => new TagViewModel
            {
                Tag = t,
                JobCount = t.ScrapingJobs.Count
            })
            .ToList();
        return View(tags);
    }

    // POST: /Tags/Create
    /// <summary>
    /// Creates a new tag with specified name and color
    /// Implements tag creation algorithm with validation
    /// </summary>
    /// <param name="name">Tag name</param>
    /// <param name="color">Tag color in hex format</param>
    /// <returns>Redirect to index with success/error message</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name, string color)
    {
        // Tag validation algorithm
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Tag name is required.";
            return RedirectToAction("Index");
        }

        // Check for duplicate tag names
        var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
        if (existingTag != null)
        {
            TempData["Error"] = "A tag with this name already exists.";
            return RedirectToAction("Index");
        }

        var tag = new Tag 
        { 
            Name = name.Trim(), 
            Color = string.IsNullOrWhiteSpace(color) ? "#6a11cb" : color 
        };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Tag created successfully.";
        return RedirectToAction("Index");
    }

    // POST: /Tags/Delete/{id}
    /// <summary>
    /// Deletes a tag and removes all its job associations
    /// Implements cascade delete for M-N relationships
    /// </summary>
    /// <param name="id">Tag ID to delete</param>
    /// <returns>Redirect to index with success/error message</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var tag = await _context.Tags
            .Include(t => t.ScrapingJobs)
            .FirstOrDefaultAsync(t => t.TagId == id);
        
        if (tag == null)
        {
            TempData["Error"] = "Tag not found.";
            return RedirectToAction("Index");
        }

        // M-N relationship cleanup algorithm - EF Core handles this automatically
        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Tag deleted successfully.";
        return RedirectToAction("Index");
    }

    /// <summary>
    /// Updates the tags assigned to a specific scraping job
    /// Implements M-N relationship management algorithm
    /// </summary>
    /// <param name="jobId">ID of the scraping job</param>
    /// <param name="selectedTags">Array of tag IDs to assign</param>
    /// <returns>Redirect to job details</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous] // Allow job owners to update their job tags
    public async Task<IActionResult> UpdateJobTags(int jobId, int[] selectedTags)
    {
        var job = await _context.ScrapingJobs
            .Include(j => j.Tags)
            .FirstOrDefaultAsync(j => j.ScrapingJobId == jobId);
        
        if (job == null) return NotFound();

        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null) return Unauthorized();

        // Authorization algorithm - admin or job owner can update tags
        if (!User.IsInRole("Admin") && job.UserId != currentUser.Id) 
            return Forbid();

        // M-N relationship update algorithm
        // 1. Clear existing tag associations
        job.Tags.Clear();
        
        // 2. Add new tag associations
        if (selectedTags != null && selectedTags.Length > 0)
        {
            var tags = await _context.Tags
                .Where(t => selectedTags.Contains(t.TagId))
                .ToListAsync();
            
            foreach (var tag in tags)
            {
                job.Tags.Add(tag);
            }
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Tags updated successfully.";
        return RedirectToAction("Details", "Jobs", new { id = jobId });
    }

    /// <summary>
    /// Gets all jobs associated with a specific tag
    /// Implements M-N relationship querying
    /// </summary>
    /// <param name="id">Tag ID</param>
    /// <returns>View with jobs using this tag</returns>
    public async Task<IActionResult> JobsByTag(int id)
    {
        var tag = await _context.Tags
            .Include(t => t.ScrapingJobs)
                .ThenInclude(j => j.User)
            .Include(t => t.ScrapingJobs)
                .ThenInclude(j => j.ScrapingResults)
            .FirstOrDefaultAsync(t => t.TagId == id);

        if (tag == null) return NotFound();

        ViewBag.Tag = tag;
        return View(tag.ScrapingJobs.ToList());
    }
} 