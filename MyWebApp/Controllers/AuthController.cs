using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.Application.DTOs.Auth;
using MyWebApp.Application.Interfaces;
using MyWebApp.ViewModels.Auth;

namespace MyWebApp.Controllers;

[AllowAnonymous]
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult AdminLogin()
    {
        return View(new AdminLoginVm());
    }

    [HttpPost]
    public async Task<IActionResult> AdminLogin(AdminLoginVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var result = await _authService.AdminLoginAsync(
            new AdminLoginRequestDto { Username = vm.Username, Password = vm.Password }
        );

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Login failed.");
            return View(vm);
        }

        // claims stored in auth cookie
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, result.Username!.Value.ToString()),
            new Claim(ClaimTypes.Role, "Admin"),
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Redirect(result.RedirectUrl);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(AdminLogin));
    }

    [HttpGet]
    public IActionResult UserLogin()
    {
        return View(new UserLoginVm());
    }

    [HttpPost]
    public async Task<IActionResult> UserLogin(UserLoginVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var result = await _authService.UserLoginAsync(
            new UserLoginRequestDto { Username = vm.Username, Password = vm.Password }
        );

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Login failed.");
            return View(vm);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, result.Username!.Value.ToString()),
            new Claim(ClaimTypes.Role, "User"),
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Redirect(result.RedirectUrl);
    }

    [HttpGet]
    public IActionResult UserRegistration()
    {
        return View(new UserRegistrationVm());
    }

    [HttpPost]
    public async Task<IActionResult> UserRegistration(UserRegistrationVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var result = await _authService.RegisterUserAsync(
            new UserRegistrationRequestDto
            {
                Name = vm.Name,
                Email = vm.Email,
                Username = vm.Username,
                Password = vm.Password,
            }
        );

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Registration failed.");
            return View(vm);
        }

        return Redirect(result.RedirectUrl);
    }
}
