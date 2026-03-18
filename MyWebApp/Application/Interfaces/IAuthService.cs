using MyWebApp.Application.DTOs.Auth;

namespace MyWebApp.Application.Interfaces;

public interface IAuthService
{
    Task<AdminLoginResultDto> AdminLoginAsync(AdminLoginRequestDto request);
    Task<UserLoginResultDto> UserLoginAsync(UserLoginRequestDto request);
    Task<UserRegistrationResultDto> RegisterUserAsync(UserRegistrationRequestDto request);
}


// This interface defines a contract for admin login that must be implemented by a service class,
// returning login result asynchronously.
