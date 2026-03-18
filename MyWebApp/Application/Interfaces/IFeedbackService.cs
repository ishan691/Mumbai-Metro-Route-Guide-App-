using MyWebApp.Application.DTOs.Feedback;

namespace MyWebApp.Application.Interfaces;

public interface IFeedbackService
{
    Task<FeedbackResultDto> SubmitAsync(FeedbackCreateDto dto);
    Task<FeedbackListResultDto> GetPagedAsync(int page, int pageSize);
}