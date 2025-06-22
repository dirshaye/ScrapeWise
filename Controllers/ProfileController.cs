using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class ProfileController : Controller
{
    private readonly AppDbContext _context;

    public ProfileController(AppDbContext context)
    {
        _context = context;
    }

    private async Task<User> GetOrCreateDefaultUserAsync()
    {
        var user = await _context.Users.Include(u => u.ConfigProfile).FirstOrDefaultAsync();
        if (user == null)
        {
            user = new User { Email = "default@example.com", PasswordHash = "..." };
            user.ConfigProfile = new ConfigProfile { DisplayName = "Default User" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        else if (user.ConfigProfile == null)
        {
            user.ConfigProfile = new ConfigProfile { DisplayName = "Default User" };
            await _context.SaveChangesAsync();
        }
        return user;
    }

    // GET: /Profile
    public async Task<IActionResult> Index()
    {
        var user = await GetOrCreateDefaultUserAsync();
        return View(user.ConfigProfile);
    }

    // POST: /Profile
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ConfigProfile profile)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.Users.Include(u => u.ConfigProfile).FirstOrDefaultAsync();
            if (user != null)
            {
                user.ConfigProfile.DisplayName = profile.DisplayName;
                user.ConfigProfile.AvatarUrl = profile.AvatarUrl;
                user.ConfigProfile.UserAgent = profile.UserAgent;
                user.ConfigProfile.DelayBetweenRequests = profile.DelayBetweenRequests;
                
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Index");
            }
        }
        return View(profile);
    }
} 