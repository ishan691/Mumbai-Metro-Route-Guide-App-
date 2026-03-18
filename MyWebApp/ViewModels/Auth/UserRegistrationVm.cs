using System.ComponentModel.DataAnnotations;

namespace MyWebApp.ViewModels.Auth;

public class UserRegistrationVm
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public int Username { get; set; }

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}