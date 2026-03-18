namespace MyWebApp.Application.DTOs.Auth;

public class UserRegistrationResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }

    // where to go after register (you can change later)
    public string RedirectUrl { get; set; } = "/Auth/UserLogin";
}
