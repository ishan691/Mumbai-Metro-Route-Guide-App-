namespace MyWebApp.Application.DTOs.Feedback;

public class FeedbackDto
{
    public int Id { get; set; }
    public string EmailId { get; set; } = string.Empty;
    public int OverallRating { get; set; }
    public int CleanlinessRating { get; set; }
    public int FacilitiesRating { get; set; }
    public int AccessibilityRating { get; set; }
    public string Suggestions { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}