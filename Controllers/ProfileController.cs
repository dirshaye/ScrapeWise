using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

[Authorize]
public class ProfileController : Controller
{
    private readonly AppDbContext _context;

    public ProfileController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var user = await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user == null) return Unauthorized();

        if (user.Profile == null)
            user.Profile = new Profile { DisplayName = "Default User" };

        return View(user.Profile);
    }

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

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Profile updated successfully!";
        return View(user.Profile);
    }

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