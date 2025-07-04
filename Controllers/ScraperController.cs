using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class ScraperController : Controller
{
    private readonly AppDbContext _context;

    public ScraperController(AppDbContext context)
    {
        _context = context;
    }

    private async Task<User> GetOrCreateDefaultUserAsync()
    {
        var user = await _context.Users.FirstOrDefaultAsync();
        if (user == null)
        {
            user = new User { Email = "default@example.com", Password = "..." };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        return user;
    }

    // GET: /Scraper/NewJob
    public IActionResult NewJob()
    {
        ViewBag.Tags = _context.Tags.ToList();
        return View();
    }

    // POST: /Scraper/NewJob
    [HttpPost]
    public async Task<IActionResult> NewJob(string targetUrl, string cssSelector, int[] selectedTags)
    {
        if (string.IsNullOrEmpty(targetUrl) || string.IsNullOrEmpty(cssSelector))
        {
            ViewBag.Tags = _context.Tags.ToList();
            ModelState.AddModelError("", "Please provide both URL and selector.");
            return View();
        }

        try
        {
            // Get the currently logged-in user
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (currentUser == null) return Unauthorized();

            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(targetUrl);

            // Save raw HTML (optional)
            System.IO.File.WriteAllText("scraped.html", doc.DocumentNode.InnerHtml);

            var nodes = doc.DocumentNode.SelectNodes($"//*[contains(@class, '{cssSelector.Replace(".", "")}')]");

            // Create and save the job
            var job = new ScrapingJob
            {
                TargetUrl = targetUrl,
                CssSelector = cssSelector,
                CreatedAt = DateTime.UtcNow,
                UserId = currentUser.Id // Assign to the logged-in user
            };

            // Assign tags
            if (selectedTags != null && selectedTags.Length > 0)
            {
                var tags = _context.Tags.Where(t => selectedTags.Contains(t.TagId)).ToList();
                foreach (var tag in tags)
                {
                    job.Tags.Add(tag);
                }
            }

            if (nodes != null && nodes.Count > 0)
            {
                foreach (var node in nodes)
                {
                    job.ScrapingResults.Add(new ScrapingResult
                    {
                        ExtractedText = node.InnerText.Trim(),
                        ScrapedAt = DateTime.UtcNow
                    });
                }
            }

            _context.ScrapingJobs.Add(job);
            await _context.SaveChangesAsync();

            if (job.ScrapingResults.Count == 0)
            {
                ViewBag.Tags = _context.Tags.ToList();
                ViewBag.Message = "No results found for the given selector. The job was saved for your records.";
                return View();
            }

            // Redirect to dashboard after successful save
            return RedirectToAction("Dashboard");
        }
        catch (Exception ex)
        {
            // Log the detailed exception
            Console.WriteLine(ex.ToString());
            ViewBag.Tags = _context.Tags.ToList();
            ViewBag.Message = "Error: An error occurred while saving the entity changes. See the inner exception for details.";
            return View();
        }
    }

    // GET: /Scraper/JobDetails/{id}
    public async Task<IActionResult> JobDetails(int id)
    {
        var job = await _context.ScrapingJobs.Include(j => j.ScrapingResults).Include(j => j.Tags).FirstOrDefaultAsync(j => j.ScrapingJobId == id);
        if (job == null) return NotFound();

        var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user == null) return Unauthorized();
        if (user.Role != "Admin" && job.UserId != user.Id) return Forbid();

        ViewBag.AllTags = _context.Tags.ToList();
        ViewBag.SelectedTagIds = job.Tags.Select(t => t.TagId).ToList();
        return View(job);
    }

    // POST: /Scraper/UpdateJobTags/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateJobTags(int id, int[] selectedTags)
    {
        var job = await _context.ScrapingJobs.Include(j => j.Tags).FirstOrDefaultAsync(j => j.ScrapingJobId == id);
        if (job == null) return NotFound();

        var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user == null) return Unauthorized();
        if (user.Role != "Admin" && job.UserId != user.Id) return Forbid();

        job.Tags.Clear();
        if (selectedTags != null && selectedTags.Length > 0)
        {
            var tags = _context.Tags.Where(t => selectedTags.Contains(t.TagId)).ToList();
            foreach (var tag in tags)
            {
                job.Tags.Add(tag);
            }
        }
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Tags updated successfully.";
        return RedirectToAction("JobDetails", new { id });
    }

    // GET: /Scraper/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user == null) return Unauthorized();

        List<ScrapingJob> jobs;
        if (user.Role == "Admin")
        {
            jobs = await _context.ScrapingJobs.Include(j => j.ScrapingResults).OrderByDescending(j => j.ScrapingJobId).ToListAsync();
        }
        else
        {
            jobs = await _context.ScrapingJobs.Include(j => j.ScrapingResults).Where(j => j.UserId == user.Id).OrderByDescending(j => j.ScrapingJobId).ToListAsync();
        }
        return View(jobs);
    }

    // POST: /Scraper/Delete/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var job = await _context.ScrapingJobs.FindAsync(id);
        if (job == null) return NotFound();

        var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user == null) return Unauthorized();
        if (user.Role != "Admin" && job.UserId != user.Id) return Forbid();

        _context.ScrapingJobs.Remove(job);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Job deleted successfully.";
        return RedirectToAction(nameof(Dashboard));
    }
}
