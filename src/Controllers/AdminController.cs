using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Controller for administrative functions and user management
/// Restricted to users with Admin role only
/// </summary>
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the AdminController
    /// </summary>
    /// <param name="context">Database context for data operations</param>
    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Displays all users in the system with their profiles
    /// </summary>
    /// <returns>View containing list of all users</returns>
    public async Task<IActionResult> Users()
    {
        var users = await _context.Users.Include(u => u.Profile).ToListAsync();
        return View(users);
    }

    /// <summary>
    /// Deletes a user account from the system
    /// Prevents admin from deleting their own account
    /// </summary>
    /// <param name="id">User ID to delete</param>
    /// <returns>Redirect to Users page or NotFound if user doesn't exist</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        // Prevent admin from deleting themselves (use Id)
        var currentUserEmail = User.Identity.Name;
        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);
        if (currentUser != null && user.Id == currentUser.Id)
        {
            TempData["Error"] = "You cannot delete your own account.";
            return RedirectToAction("Users");
        }
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        TempData["Success"] = "User deleted successfully.";
        return RedirectToAction("Users");
    }

    // POST: /Admin/ToggleActiveUser
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActiveUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        // Prevent admin from deactivating themselves
        var currentUserEmail = User.Identity.Name;
        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);
        if (currentUser != null && user.Id == currentUser.Id)
        {
            TempData["Error"] = "You cannot deactivate your own account.";
            return RedirectToAction("Users");
        }
        user.IsActive = !user.IsActive;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        TempData["Success"] = $"User {(user.IsActive ? "activated" : "deactivated")} successfully.";
        return RedirectToAction("Users");
    }
} 