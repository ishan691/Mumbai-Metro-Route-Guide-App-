namespace MyWebApp.Application.DTOs.Auth;

public class UserLoginRequestDto
{
    public int Username { get; set; }
    public string Password { get; set; } = string.Empty;
}
