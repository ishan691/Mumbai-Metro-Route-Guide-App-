using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Application.DTOs.Auth;
using MyWebApp.Application.Interfaces;
using MyWebApp.Infrastructure.Data;
using MyWebApp.Infrastructure.Data.Entities;

namespace MyWebApp.Application.Services;

public class AuthService : IAuthService
{
    private readonly SiteDbContext _db;
    private readonly IPasswordHasher<Admin> _adminHasher;
    private readonly IPasswordHasher<User> _userHasher;

    public AuthService(
        SiteDbContext db,
        IPasswordHasher<Admin> adminHasher,
        IPasswordHasher<User> userHasher
    )
    {
        _db = db;
        _adminHasher = adminHasher;
        _userHasher = userHasher;
    }

    public async Task<AdminLoginResultDto> AdminLoginAsync(AdminLoginRequestDto request)
    {
        var admin = await _db.Admins.FirstOrDefaultAsync(a => a.Username == request.Username);

        if (admin is null)
        {
            return new AdminLoginResultDto
            {
                Success = false,
                ErrorMessage = "Invalid username or password.",
            };
        }

        var stored = admin.PasswordHash ?? string.Empty;
        bool looksHashed = stored.StartsWith("AQAAAA", StringComparison.Ordinal);

        if (looksHashed)
        {
            var verify = _adminHasher.VerifyHashedPassword(admin, stored, request.Password);
            if (verify == PasswordVerificationResult.Failed)
                return new AdminLoginResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid username or password.",
                };
        }
        else
        {
            if (!string.Equals(stored, request.Password, StringComparison.Ordinal))
                return new AdminLoginResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid username or password.",
                };

            admin.PasswordHash = _adminHasher.HashPassword(admin, request.Password);
            await _db.SaveChangesAsync();
        }

        return new AdminLoginResultDto
        {
            Success = true,
            Username = admin.Username,
            RedirectUrl = "/adminDashboard.html",
        };
    }

    public async Task<UserLoginResultDto> UserLoginAsync(UserLoginRequestDto request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user is null)
        {
            return new UserLoginResultDto
            {
                Success = false,
                ErrorMessage = "Invalid username or password.",
            };
        }

        var stored = user.PasswordHash ?? string.Empty;
        bool looksHashed = stored.StartsWith("AQAAAA", StringComparison.Ordinal);

        if (looksHashed)
        {
            var verify = _userHasher.VerifyHashedPassword(user, stored, request.Password);
            if (verify == PasswordVerificationResult.Failed)
                return new UserLoginResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid username or password.",
                };
        }
        else
        {
            // legacy support (if your DB currently has plain passwords)
            if (!string.Equals(stored, request.Password, StringComparison.Ordinal))
                return new UserLoginResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid username or password.",
                };

            // upgrade to hash
            user.PasswordHash = _userHasher.HashPassword(user, request.Password);
            await _db.SaveChangesAsync();
        }

        return new UserLoginResultDto
        {
            Success = true,
            Username = user.Username,
            RedirectUrl = "/userInterface.html",
        };
    }

    public async Task<UserRegistrationResultDto> RegisterUserAsync(
        UserRegistrationRequestDto request
    )
    {
        // 1) Duplicate checks (VERY common interview topic)
        var usernameExists = await _db.Users.AnyAsync(u => u.Username == request.Username);
        if (usernameExists)
        {
            return new UserRegistrationResultDto
            {
                Success = false,
                ErrorMessage = "User ID already exists.",
            };
        }

        var emailExists = await _db.Users.AnyAsync(u => u.Email == request.Email);
        if (emailExists)
        {
            return new UserRegistrationResultDto
            {
                Success = false,
                ErrorMessage = "Email already exists.",
            };
        }

        // 2) Create entity
        var user = new User
        {
            Username = request.Username,
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
        };

        // 3) Hash password
        user.PasswordHash = _userHasher.HashPassword(user, request.Password);

        // 4) Save
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new UserRegistrationResultDto { Success = true, RedirectUrl = "/Auth/UserLogin" };
    }
}
