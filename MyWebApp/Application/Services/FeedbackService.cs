using Microsoft.EntityFrameworkCore;
using MyWebApp.Application.DTOs.Feedback;
using MyWebApp.Application.Interfaces;
using MyWebApp.Infrastructure.Data;
using MyWebApp.Infrastructure.Data.Entities;

namespace MyWebApp.Application.Services;

public class FeedbackService : IFeedbackService
{
    private readonly SiteDbContext _db;

    public FeedbackService(SiteDbContext db)
    {
        _db = db;
    }

    public async Task<FeedbackResultDto> SubmitAsync(FeedbackCreateDto dto)
    {
        try
        {
            var feedback = new Feedback
            {
                EmailId = dto.EmailId.Trim(),
                OverallRating = dto.OverallRating,
                CleanlinessRating = dto.CleanlinessRating,
                FacilitiesRating = dto.FacilitiesRating,
                AccessibilityRating = dto.AccessibilityRating,
                Suggestions = dto.Suggestions.Trim(),
                CreatedAt = DateTime.UtcNow,
            };

            _db.Feedbacks.Add(feedback);
            await _db.SaveChangesAsync();

            return new FeedbackResultDto { Success = true };
        }
        catch (Exception ex)
        {
            return new FeedbackResultDto { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<FeedbackListResultDto> GetPagedAsync(int page, int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 5 or > 100 ? 10 : pageSize;

        var query = _db.Feedbacks.AsNoTracking();

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new FeedbackDto
            {
                Id = x.Id,
                EmailId = x.EmailId,
                OverallRating = x.OverallRating,
                CleanlinessRating = x.CleanlinessRating,
                FacilitiesRating = x.FacilitiesRating,
                AccessibilityRating = x.AccessibilityRating,
                Suggestions = x.Suggestions,
                CreatedAt = x.CreatedAt,
            })
            .ToListAsync();

        return new FeedbackListResultDto
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
        };
    }
}
