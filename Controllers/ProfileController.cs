using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class ProfileController : Controller
{
    private readonly AppDbContext _context;

    public ProfileController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync();
        if (user == null)
            return NotFound();

        if (user.Profile == null)
            user.Profile = new Profile { DisplayName = "Default User" };

        return View(user.Profile);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(Profile profile)
    {
        var user = await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync();
        if (user == null)
            return NotFound();

        if (user.Profile == null)
            user.Profile = new Profile();

        user.Profile.DisplayName = profile.DisplayName;
        user.Profile.AvatarUrl = profile.AvatarUrl;
        user.Profile.UserAgent = profile.UserAgent;
        user.Profile.DelayBetweenRequests = profile.DelayBetweenRequests;

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Profile updated successfully!";
        return View(user.Profile);
    }
} 