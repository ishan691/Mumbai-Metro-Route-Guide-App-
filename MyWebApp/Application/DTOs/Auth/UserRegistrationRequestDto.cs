namespace MyWebApp.Application.DTOs.Auth;

public class UserRegistrationRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Username { get; set; }
    public string Password { get; set; } = string.Empty;
}
