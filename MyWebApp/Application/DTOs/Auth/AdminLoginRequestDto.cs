namespace MyWebApp.Application.DTOs.Auth;

public class AdminLoginRequestDto
{
    public int Username { get; set; }
    public string Password { get; set; } = string.Empty;
}