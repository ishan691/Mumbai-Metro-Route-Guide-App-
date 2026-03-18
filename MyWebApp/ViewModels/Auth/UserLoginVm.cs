using System.ComponentModel.DataAnnotations;

namespace MyWebApp.ViewModels.Auth;

public class UserLoginVm
{
    [Required]
    public int Username { get; set; }

    [Required]
    public string Password { get; set; } = string.Empty;
}