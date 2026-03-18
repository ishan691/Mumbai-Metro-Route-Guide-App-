namespace MyWebApp.Application.DTOs.Auth;

public class AdminLoginResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }

    // used by controller for claims
    public int? Username { get; set; }

    // where to go after login
    public string RedirectUrl { get; set; } = "/adminDashboard.html";
}
