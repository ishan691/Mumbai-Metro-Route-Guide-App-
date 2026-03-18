using System.ComponentModel.DataAnnotations;

namespace MyWebApp.ViewModels.Auth;

public class AdminLoginVm
{
    [Required]
    public int Username { get; set; }

    [Required]
    public string Password { get; set; } = string.Empty;
}
