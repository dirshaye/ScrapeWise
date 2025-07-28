using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Controller for authentication and account management
/// Works in conjunction with ASP.NET Core Identity pages
/// </summary>
public class AccountController : Controller
{
    private readonly SignInManager<MyUser> _signInManager;

    /// <summary>
    /// Initializes a new instance of the AccountController
    /// </summary>
    /// <param name="signInManager">Identity sign-in manager for authentication operations</param>
    public AccountController(SignInManager<MyUser> signInManager)
    {
        _signInManager = signInManager;
    }

    /// <summary>
    /// Signs out the current user and redirects to home page
    /// </summary>
    /// <returns>Redirect to home page</returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Redirects to the Identity login page
    /// </summary>
    /// <returns>Redirect to Identity login page</returns>
    public IActionResult Login()
    {
        return Redirect("/Identity/Account/Login");
    }

    /// <summary>
    /// Redirects to the Identity registration page
    /// </summary>
    /// <returns>Redirect to Identity registration page</returns>
    public IActionResult Register()
    {
        return Redirect("/Identity/Account/Register");
    }
} 