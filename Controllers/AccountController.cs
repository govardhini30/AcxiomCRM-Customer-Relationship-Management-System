using AcxiomCRM.Models;
using AcxiomCRM.Services;
using AcxiomCRM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AcxiomCRM.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AuditService _audit;

    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AuditService audit)
    {
        _signInManager = signInManager; _userManager = userManager; _audit = audit;
    }

    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null) => View(new LoginVM { });

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null || !user.IsActive)
        {
            ModelState.AddModelError("", "Invalid login attempt.");
            await _audit.LogAsync("LoginFailed", "Authentication", null, result: "Failure", details: "Invalid user or inactive account.");
            return View(model);
        }
        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
        await _audit.LogAsync(result.Succeeded ? "Login" : "LoginFailed", "Authentication", user.Id, result: result.Succeeded ? "Success" : "Failure");
        if (result.Succeeded) return LocalRedirect(returnUrl ?? Url.Action("Index", "Dashboard")!);
        if (result.IsLockedOut) ModelState.AddModelError("", "Account is temporarily locked. Try again later.");
        else ModelState.AddModelError("", "Invalid login attempt.");
        return View(model);
    }

    [AllowAnonymous]
    public IActionResult Register() => View(new RegisterVM());

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVM model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = new ApplicationUser { Name = model.Name, UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
            return View(model);
        }
        await _userManager.AddToRoleAsync(user, "SalesExecutive");
        await _signInManager.SignInAsync(user, false);
        await _audit.LogAsync("Register", "User", user.Id, newValue: new { user.Email, Role = "SalesExecutive" });
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();
}

