using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ScrapeWise_Intelligent_Web_Scraping_Dashboard_ASP.NET_Core_MVC_.Models;

namespace ScrapeWise_Intelligent_Web_Scraping_Dashboard_ASP.NET_Core_MVC_.Controllers;

/// <summary>
/// Home controller responsible for public pages and application entry points
/// Follows MVC pattern for content display and navigation
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Initializes a new instance of the HomeController
    /// </summary>
    /// <param name="logger">Logger for tracking and debugging</param>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Displays the application home page
    /// </summary>
    /// <returns>Home page view</returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Displays the privacy policy page
    /// </summary>
    /// <returns>Privacy policy view</returns>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Displays the application documentation
    /// </summary>
    /// <returns>Documentation view</returns>
    public IActionResult Docs()
    {
        return View();
    }

    /// <summary>
    /// Displays the help and support page
    /// </summary>
    /// <returns>Help page view</returns>
    public IActionResult Help()
    {
        return View();
    }

    /// <summary>
    /// Handles application errors and displays error page
    /// Implements proper error handling for production use
    /// </summary>
    /// <returns>Error page with request tracking information</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
