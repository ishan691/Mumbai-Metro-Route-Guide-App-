using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Infrastructure.Data.Entities;

public class RouteLine
{
    public uint RouteId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}