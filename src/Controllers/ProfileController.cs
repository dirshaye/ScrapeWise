using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

/// <summary>
/// Controller for managing user profiles and preferences
/// Handles user profile display and updates
/// </summary>
[Authorize]
public class ProfileController : Controller
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the ProfileController
    /// </summary>
    /// <param name="context">Database context for data operations</param>
    public ProfileController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Displays the current user's profile information
    /// Creates a default profile if none exists
    /// </summary>
    /// <returns>Profile view with user's profile data</returns>
    public async Task<IActionResult> Index()
    {
        var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var user = await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user == null) return Unauthorized();

        if (user.Profile == null)
            user.Profile = new Profile { DisplayName = "Default User" };

        return View(user.Profile);
    }

    /// <summary>
    /// Updates the current user's profile information
    /// </summary>
    /// <param name="profile">Profile data from the form</param>
    /// <returns>Updated profile view or error page</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(Profile profile)
    {
        var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var user = await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user == null) return Unauthorized();

        if (user.Profile == null)
            user.Profile = new Profile();

        user.Profile.DisplayName = profile.DisplayName;
        user.Profile.AvatarUrl = profile.AvatarUrl;
        user.Profile.UserAgent = string.IsNullOrWhiteSpace(profile.UserAgent) ? "Mozilla/5.0 (compatible; ScrapeWiseBot/1.0)" : profile.UserAgent;
        user.Profile.DelayBetweenRequests = profile.DelayBetweenRequests;
        user.UserName = profile.DisplayName;
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Profile updated successfully!";
        return View(user.Profile);
    }

    /// <summary>
    /// Deactivates the current user's account and signs them out
    /// </summary>
    /// <returns>Redirect to login page with confirmation message</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeactivateAccount()
    {
        var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user == null) return Unauthorized();
        user.IsActive = false;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        await HttpContext.SignOutAsync();
        TempData["SuccessMessage"] = "Your account has been deactivated.";
        return RedirectToAction("Login", "Account");
    }

    /// <summary>
    /// Permanently deletes the current user's account and all associated data
    /// Signs out the user and redirects to registration page
    /// </summary>
    /// <returns>Redirect to registration page with confirmation message</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount()
    {
        var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user == null) return Unauthorized();
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        await HttpContext.SignOutAsync();
        TempData["SuccessMessage"] = "Your account has been deleted.";
        return RedirectToAction("Register", "Account");
    }
} 