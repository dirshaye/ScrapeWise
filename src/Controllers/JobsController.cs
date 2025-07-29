using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ScrapeWise_Intelligent_Web_Scraping_Dashboard_ASP.NET_Core_MVC_.Controllers;

/// <summary>
/// Controller responsible for managing scraping jobs
/// Handles CRUD operations for ScrapingJob entities only
/// Uses TagService for tag operations to maintain separation of concerns
/// </summary>
[Authorize]
public class JobsController : Controller
{
    private readonly AppDbContext _context;
    private readonly IHubContext<ScrapingHub> _hubContext;
    private readonly ITagService _tagService;

    /// <summary>
    /// Initializes a new instance of the JobsController
    /// </summary>
    /// <param name="context">Database context for job operations</param>
    /// <param name="hubContext">SignalR hub context for real-time notifications</param>
    /// <param name="tagService">Tag service for tag-related operations</param>
    public JobsController(AppDbContext context, IHubContext<ScrapingHub> hubContext, ITagService tagService)
    {
        _context = context;
        _hubContext = hubContext;
        _tagService = tagService;
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
    /// Displays the dashboard with user's scraping jobs
    /// Implements role-based access: Admins see all jobs, regular users see only their own
    /// </summary>
    /// <returns>View with list of scraping jobs</returns>
    public async Task<IActionResult> Index()
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null) return Unauthorized();

        // Role-based job filtering algorithm
        List<ScrapingJob> jobs;
        if (User.IsInRole("Admin"))
        {
            // Admin users can view all jobs with full details
            jobs = await _context.ScrapingJobs
                .Include(j => j.ScrapingResults)
                .Include(j => j.User)
                .Include(j => j.Tags)
                .OrderByDescending(j => j.ScrapingJobId)
                .ToListAsync();
        }
        else
        {
            // Regular users can only view their own jobs
            jobs = await _context.ScrapingJobs
                .Include(j => j.ScrapingResults)
                .Include(j => j.User)
                .Include(j => j.Tags)
                .Where(j => j.UserId == currentUser.Id)
                .OrderByDescending(j => j.ScrapingJobId)
                .ToListAsync();
        }

        return View(jobs);
    }

    /// <summary>
    /// Displays the form for creating a new scraping job
    /// Loads available tags for user selection
    /// </summary>
    /// <returns>View with available tags</returns>
    public async Task<IActionResult> Create()
    {
        ViewBag.Tags = await _tagService.GetAllTagsAsync();
        return View();
    }

    /// <summary>
    /// Alias for Create action - supports /Jobs/NewJob route
    /// </summary>
    /// <returns>View with available tags</returns>
    public async Task<IActionResult> NewJob()
    {
        ViewBag.Tags = await _tagService.GetAllTagsAsync();
        return View("Create"); // Use the same Create view
    }

    /// <summary>
    /// Creates a new scraping job and performs the scraping operation
    /// Implements web scraping algorithm using HtmlAgilityPack with real-time SignalR notifications
    /// </summary>
    /// <param name="targetUrl">URL to scrape</param>
    /// <param name="cssSelector">CSS selector for target elements</param>
    /// <param name="selectedTags">Array of tag IDs to assign to the job</param>
    /// <returns>Redirect to dashboard on success, or view with errors</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string targetUrl, string cssSelector, int[] selectedTags)
    {
        // Input validation algorithm
        if (string.IsNullOrEmpty(targetUrl) || string.IsNullOrEmpty(cssSelector))
        {
            ViewBag.Tags = _context.Tags.ToList();
            ModelState.AddModelError("", "Please provide both URL and selector.");
            return View();
        }

        try
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null) return Unauthorized();

            // Create new scraping job entity first to get ID for SignalR
            var job = new ScrapingJob
            {
                TargetUrl = targetUrl,
                CssSelector = cssSelector,
                CreatedAt = DateTime.UtcNow,
                UserId = currentUser.Id
            };

            // Tag assignment algorithm - Many-to-Many relationship handling  
            if (selectedTags != null && selectedTags.Length > 0)
            {
                var tags = await _tagService.GetTagsByIdsAsync(selectedTags);
                foreach (var tag in tags)
                {
                    job.Tags.Add(tag);
                }
            }

            // Save job to get the ID for SignalR notifications
            _context.ScrapingJobs.Add(job);
            await _context.SaveChangesAsync();

            // SignalR Notification: Job Started
            await _hubContext.Clients.All.SendAsync("JobStarted", job.ScrapingJobId, targetUrl, currentUser.UserName ?? currentUser.Email);

            // Web scraping algorithm using HtmlAgilityPack
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(targetUrl);

            // CSS selector parsing algorithm - handles various CSS selector types
            HtmlNodeCollection nodes = null;
            
            if (cssSelector.StartsWith("."))
            {
                // Class selector: .className
                var className = cssSelector.Substring(1);
                nodes = doc.DocumentNode.SelectNodes($"//*[contains(concat(' ', normalize-space(@class), ' '), ' {className} ')]");
            }
            else if (cssSelector.StartsWith("#"))
            {
                // ID selector: #idName
                var idName = cssSelector.Substring(1);
                nodes = doc.DocumentNode.SelectNodes($"//*[@id='{idName}']");
            }
            else
            {
                // Element selector: h1, p, div, etc.
                nodes = doc.DocumentNode.SelectNodes($"//{cssSelector}");
            }

            // Data extraction algorithm with real-time progress updates
            int resultCount = 0;
            if (nodes != null && nodes.Count > 0)
            {
                foreach (var node in nodes)
                {
                    // Text extraction and normalization
                    var extractedText = node.InnerText.Trim();
                    if (!string.IsNullOrWhiteSpace(extractedText))
                    {
                        var result = new ScrapingResult
                        {
                            ExtractedText = extractedText,
                            ScrapedAt = DateTime.UtcNow,
                            ScrapingJobId = job.ScrapingJobId
                        };
                        
                        job.ScrapingResults.Add(result);
                        resultCount++;

                        // SignalR Notification: New Result Found
                        await _hubContext.Clients.All.SendAsync("NewResult", job.ScrapingJobId, extractedText, resultCount);

                        // Add small delay to make progress visible (remove in production)
                        await Task.Delay(100);
                    }
                }
            }

            // Save all results
            await _context.SaveChangesAsync();

            // SignalR Notification: Job Completed
            if (job.ScrapingResults.Count > 0)
            {
                await _hubContext.Clients.All.SendAsync("JobCompleted", job.ScrapingJobId, job.ScrapingResults.Count);
                TempData["SuccessMessage"] = $"Scraping job created successfully with {job.ScrapingResults.Count} results.";
            }
            else
            {
                await _hubContext.Clients.All.SendAsync("JobCompleted", job.ScrapingJobId, 0);
                ViewBag.Tags = _context.Tags.ToList();
                ViewBag.Message = "No results found for the given selector. The job was saved for your records.";
                return View();
            }

            return RedirectToAction("Index");
        }
        catch (HttpRequestException httpEx)
        {
            // SignalR Notification: Job Failed
            await _hubContext.Clients.All.SendAsync("JobFailed", 0, $"Network error: {httpEx.Message}");
            
            ViewBag.Tags = _context.Tags.ToList();
            ViewBag.Message = $"Failed to access the website: {httpEx.Message}. Please check the URL and try again.";
            return View();
        }
        catch (Exception ex)
        {
            // SignalR Notification: Job Failed
            await _hubContext.Clients.All.SendAsync("JobFailed", 0, ex.Message);
            
            ViewBag.Tags = _context.Tags.ToList();
            ViewBag.Message = $"Error occurred while creating the job: {ex.Message}";
            return View();
        }
    }

    /// <summary>
    /// Displays detailed information about a specific scraping job
    /// Implements authorization check - users can only view their own jobs
    /// </summary>
    /// <param name="id">Job ID</param>
    /// <returns>View with job details and results</returns>
    public async Task<IActionResult> Details(int id)
    {
        var job = await _context.ScrapingJobs
            .Include(j => j.ScrapingResults)
            .Include(j => j.Tags)
            .Include(j => j.User)
            .FirstOrDefaultAsync(j => j.ScrapingJobId == id);

        if (job == null) return NotFound();

        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null) return Unauthorized();

        // Authorization algorithm - role-based access control
        if (!User.IsInRole("Admin") && job.UserId != currentUser.Id) 
            return Forbid();

        ViewBag.AllTags = _context.Tags.ToList();
        ViewBag.SelectedTagIds = job.Tags.Select(t => t.TagId).ToList();
        return View(job);
    }

    /// <summary>
    /// Alias for Details action - supports /Jobs/JobDetails/{id} route
    /// </summary>
    /// <param name="id">Job ID</param>
    /// <returns>View with job details and results</returns>
    public async Task<IActionResult> JobDetails(int id)
    {
        var job = await _context.ScrapingJobs
            .Include(j => j.ScrapingResults)
            .Include(j => j.Tags)
            .Include(j => j.User)
            .FirstOrDefaultAsync(j => j.ScrapingJobId == id);

        if (job == null) return NotFound();

        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null) return Unauthorized();

        // Authorization algorithm - role-based access control
        if (!User.IsInRole("Admin") && job.UserId != currentUser.Id) 
            return Forbid();

        ViewBag.AllTags = _context.Tags.ToList();
        ViewBag.SelectedTagIds = job.Tags.Select(t => t.TagId).ToList();
        
        // Explicitly return the Details view to avoid view name confusion
        return View("Details", job);
    }

    /// <summary>
    /// Deletes a scraping job and all associated results
    /// Implements cascade delete for 1-N relationship with ScrapingResults
    /// </summary>
    /// <param name="id">Job ID to delete</param>
    /// <returns>Redirect to index</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var job = await _context.ScrapingJobs.FindAsync(id);
        if (job == null) return NotFound();

        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null) return Unauthorized();

        // Authorization algorithm - only owners and admins can delete
        if (!User.IsInRole("Admin") && job.UserId != currentUser.Id) 
            return Forbid();

        // Cascade delete algorithm - EF Core automatically deletes related results
        _context.ScrapingJobs.Remove(job);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Job deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
