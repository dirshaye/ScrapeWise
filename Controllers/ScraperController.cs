using Microsoft.AspNetCore.Mvc;

namespace ScrapeWise_Intelligent_Web_Scraping_Dashboard_ASP.NET_Core_MVC_.Controllers;

/// <summary>
/// Compatibility controller for old Scraper routes
/// Redirects to new controller structure (Jobs, Tags, Results)
/// This maintains backward compatibility during transition
/// 
/// ALGORITHMIC PURPOSE: Implements URL redirection algorithm for legacy route management
/// </summary>
public class ScraperController : Controller
{
    /// <summary>
    /// Redirects Dashboard to Jobs controller Index action
    /// 
    /// ALGORITHM: HTTP 302 Redirect Algorithm
    /// 1. Accept incoming request to /Scraper/Dashboard
    /// 2. Generate redirect response to /Jobs/Index
    /// 3. Preserve request context and authentication state
    /// 4. Return ActionResult that triggers browser redirect
    /// 
    /// TIME COMPLEXITY: O(1) - Constant time redirect
    /// SPACE COMPLEXITY: O(1) - No additional memory allocation
    /// </summary>
    /// <returns>Redirect to Jobs/Index</returns>
    public IActionResult Dashboard()
    {
        // Redirect algorithm - maintains SEO and user experience during refactoring
        return RedirectToAction("Index", "Jobs");
    }

    /// <summary>
    /// Redirects NewJob to Jobs controller Create action
    /// 
    /// ALGORITHM: Route Translation Algorithm
    /// 1. Map legacy route /Scraper/NewJob → /Jobs/Create
    /// 2. Preserve HTTP method (GET) and authentication context
    /// 3. Maintain query parameters if any exist
    /// 
    /// TIME COMPLEXITY: O(1) - Direct route mapping
    /// SPACE COMPLEXITY: O(1) - No data transformation required
    /// </summary>
    /// <returns>Redirect to Jobs/Create</returns>
    public IActionResult NewJob()
    {
        // Legacy route mapping algorithm - ensures backward compatibility
        return RedirectToAction("Create", "Jobs");
    }

    /// <summary>
    /// Redirects JobDetails to Jobs controller Details action
    /// 
    /// ALGORITHM: Parameter Preservation Redirect Algorithm
    /// 1. Accept incoming request with job ID parameter
    /// 2. Validate parameter existence (ASP.NET handles null/invalid IDs)
    /// 3. Create anonymous object to preserve route values
    /// 4. Generate redirect with parameter mapping: { id = originalId }
    /// 
    /// TIME COMPLEXITY: O(1) - Single parameter mapping operation
    /// SPACE COMPLEXITY: O(1) - Creates one anonymous object for route values
    /// </summary>
    /// <param name="id">Job ID parameter to preserve in redirect</param>
    /// <returns>Redirect to Jobs/Details with preserved ID</returns>
    public IActionResult JobDetails(int id)
    {
        // Parameter preservation algorithm - maintains data flow through redirects
        return RedirectToAction("Details", "Jobs", new { id });
    }

    /// <summary>
    /// Redirects ExportJobCsv to Results controller ExportCsv action
    /// 
    /// ALGORITHM: Cross-Controller Parameter Mapping Algorithm
    /// 1. Accept jobId parameter from legacy Scraper route
    /// 2. Map parameter name from 'jobId' to 'jobId' (same name)
    /// 3. Redirect to Results controller (different domain responsibility)
    /// 4. Preserve authentication and authorization context
    /// 
    /// PURPOSE: Separates export functionality from job management
    /// TIME COMPLEXITY: O(1) - Direct parameter mapping
    /// SPACE COMPLEXITY: O(1) - Single route value object creation
    /// </summary>
    /// <param name="jobId">Job ID for CSV export operation</param>
    /// <returns>Redirect to Results/ExportCsv with preserved jobId</returns>
    public IActionResult ExportJobCsv(int jobId)
    {
        // Cross-controller responsibility mapping algorithm
        return RedirectToAction("ExportCsv", "Results", new { jobId });
    }

    /// <summary>
    /// Redirects Delete to Jobs controller Delete action
    /// 
    /// ALGORITHM: HTTP Method Preservation Redirect Algorithm
    /// 1. Accept POST request with job ID parameter
    /// 2. Preserve HTTP POST method semantics through redirect
    /// 3. Maintain CSRF token validation chain
    /// 4. Map to Jobs controller for proper entity responsibility
    /// 
    /// SECURITY NOTE: POST redirects maintain form data and CSRF protection
    /// TIME COMPLEXITY: O(1) - Direct method call redirection
    /// SPACE COMPLEXITY: O(1) - No additional data structures
    /// </summary>
    /// <param name="id">Job ID to delete</param>
    /// <returns>Redirect to Jobs/Delete with preserved ID and POST semantics</returns>
    [HttpPost]
    public IActionResult Delete(int id)
    {
        // HTTP method preservation algorithm - maintains REST semantics
        return RedirectToAction("Delete", "Jobs", new { id });
    }

    /// <summary>
    /// Redirects UpdateJobTags to Tags controller UpdateJobTags action
    /// 
    /// ALGORITHM: Tag Management Redirect Algorithm
    /// 1. Accept POST request with jobId and selectedTags parameters
    /// 2. Preserve form data for tag update operation
    /// 3. Redirect to Tags controller for proper tag management responsibility
    /// 4. Maintain CSRF token and authentication context
    /// 
    /// TIME COMPLEXITY: O(1) - Direct parameter mapping redirect
    /// SPACE COMPLEXITY: O(1) - Route value object creation
    /// </summary>
    /// <param name="jobId">Job ID for tag update</param>
    /// <param name="selectedTags">Array of selected tag IDs</param>
    /// <returns>Redirect to Tags/UpdateJobTags with preserved parameters</returns>
    [HttpPost]
    public IActionResult UpdateJobTags(int jobId, int[] selectedTags)
    {
        // Cross-controller responsibility mapping for tag management
        return RedirectToAction("UpdateJobTags", "Tags", new { jobId, selectedTags });
    }
}
