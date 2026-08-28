namespace MyWebApp.Application.DTOs.Auth;

public class UserLoginResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }

    public int? Username { get; set; }

    public string RedirectUrl { get; set; } = "/userInterface.html";
}
