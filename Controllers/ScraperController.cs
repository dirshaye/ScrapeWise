using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

public class ScraperController : Controller
{
    private readonly AppDbContext _context;

    public ScraperController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Scraper/NewJob
    public IActionResult NewJob()
    {
        return View();
    }

    // POST: /Scraper/NewJob
    [HttpPost]
    public async Task<IActionResult> NewJob(string targetUrl, string cssSelector)
    {
        if (string.IsNullOrEmpty(targetUrl) || string.IsNullOrEmpty(cssSelector))
        {
            ModelState.AddModelError("", "Please provide both URL and selector.");
            return View();
        }

        try
        {
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
                CreatedAt = DateTime.Now,
                // UserId = ... // Set this if/when you have authentication
            };

            if (nodes != null && nodes.Count > 0)
            {
                foreach (var node in nodes)
                {
                    job.ScrapingResults.Add(new ScrapingResult
                    {
                        ExtractedText = node.InnerText.Trim(),
                        ScrapedAt = DateTime.Now
                    });
                }
            }

            _context.ScrapingJobs.Add(job);
            await _context.SaveChangesAsync();

            if (job.ScrapingResults.Count == 0)
            {
                ViewBag.Message = "No results found for the given selector. The job was saved for your records.";
                return View();
            }

            // Redirect to dashboard after successful save
            return RedirectToAction("Dashboard");
        }
        catch (Exception ex)
        {
            ViewBag.Message = "Error: " + ex.Message;
            return View();
        }
    }

    // GET: /Scraper/JobDetails/{id}
    public async Task<IActionResult> JobDetails(int id)
    {
        var job = await _context.ScrapingJobs
            .Include(j => j.ScrapingResults)
            .FirstOrDefaultAsync(j => j.ScrapingJobId == id);

        if (job == null) return NotFound();

        return View(job);
    }

    // GET: /Scraper/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        var jobs = await _context.ScrapingJobs
            .Include(j => j.ScrapingResults)
            .OrderByDescending(j => j.ScrapingJobId)
            .ToListAsync();
        return View(jobs);
    }
}
