namespace MyWebApp.Infrastructure.Data.Entities;

public class Admin
{
    public int Username { get; set; }

    public string PasswordHash { get; set; } = string.Empty;
}
