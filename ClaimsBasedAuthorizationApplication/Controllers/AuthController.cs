using System.Security.Claims;
using ClaimsBasesAuthorizationApplication.Data;
using ClaimsBasesAuthorizationApplication.Dtos;
using ClaimsBasesAuthorizationApplication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClaimsBasesAuthorizationApplication.Controllers;

public class AuthController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthController(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        if (!ModelState.IsValid) return View();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        if (_passwordHasher.VerifyHashedPassword(user, user.HashedPassword, loginDto.Password) ==
            PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        var claims = new List<Claim>
        {
            new("Id", user.Id.ToString()),
            new("Email", user.Email)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties();

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), authProperties);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegistrationDto registrationDto)
    {
        if (!ModelState.IsValid) return View();

        if (UserExists(registrationDto.Email))
        {
            ModelState.AddModelError(nameof(RegistrationDto.Email), "User does already exist.");
            return View();
        }

        var user = new User
        {
            Email = registrationDto.Email
        };

        user.HashedPassword = _passwordHasher.HashPassword(user, registrationDto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Login", "Auth");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private bool UserExists(string email)
    {
        return _context.Users.Any(u => u.Email == email);
    }
}