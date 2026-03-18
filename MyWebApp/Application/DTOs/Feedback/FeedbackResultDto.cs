namespace MyWebApp.Application.DTOs.Feedback;

public class FeedbackResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }

    // public string RedirectUrl { get; set; } = "/Auth/UserLogin";
}