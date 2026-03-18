using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Infrastructure.Data.Entities;

public class Feedback
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    [EmailAddress]
    public string EmailId { get; set; } = string.Empty;

    [Range(1, 5)]
    public int OverallRating { get; set; }

    [Range(1, 5)]
    public int CleanlinessRating { get; set; }

    [Range(1, 5)]
    public int FacilitiesRating { get; set; }

    [Range(1, 5)]
    public int AccessibilityRating { get; set; }

    [Required]
    [MaxLength(500)]
    public string Suggestions { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}