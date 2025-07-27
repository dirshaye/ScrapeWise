using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

public class AccountController : Controller
{
    private readonly SignInManager<MyUser> _signInManager;

    public AccountController(SignInManager<MyUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // Redirect to Identity pages
    public IActionResult Login()
    {
        return Redirect("/Identity/Account/Login");
    }

    public IActionResult Register()
    {
        return Redirect("/Identity/Account/Register");
    }
} 