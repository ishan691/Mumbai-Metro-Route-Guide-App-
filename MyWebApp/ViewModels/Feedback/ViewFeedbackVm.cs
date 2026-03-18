using MyWebApp.Application.DTOs.Feedback;

namespace MyWebApp.ViewModels.Feedback;

public class ViewFeedbackVm
{
    public IReadOnlyList<FeedbackDto> Feedbacks { get; set; } = Array.Empty<FeedbackDto>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
