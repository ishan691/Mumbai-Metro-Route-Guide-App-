using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.Application.DTOs.Feedback;
using MyWebApp.Application.Interfaces;
using MyWebApp.ViewModels.Feedback;

namespace MyWebApp.Controllers;

[AllowAnonymous]
public class FeedbackController : Controller
{
    private readonly IFeedbackService _feedbackService;

    public FeedbackController(IFeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    [HttpGet]
    public IActionResult FeedbackForm()
    {
        return View(new FeedbackFormVm());
    }

    [HttpPost]
    public async Task<IActionResult> FeedbackForm(FeedbackFormVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var result = await _feedbackService.SubmitAsync(
            new FeedbackCreateDto
            {
                EmailId = vm.EmailId,
                OverallRating = vm.OverallRating,
                CleanlinessRating = vm.CleanlinessRating,
                FacilitiesRating = vm.FacilitiesRating,
                AccessibilityRating = vm.AccessibilityRating,
                Suggestions = vm.Suggestions,
            }
        );

        if (!result.Success)
        {
            ModelState.AddModelError(
                string.Empty,
                result.ErrorMessage ?? "Feedback submission failed."
            );
            return View(vm);
        }

        return RedirectToAction(nameof(ThankYou));
    }

    [HttpGet]
    public async Task<IActionResult> ViewFeedback(int page = 1, int pageSize = 10)
    {
        var result = await _feedbackService.GetPagedAsync(page, pageSize);

        var vm = new ViewFeedbackVm
        {
            Feedbacks = result.Items,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
        };

        return View(vm);
    }

    [HttpGet]
    public IActionResult ThankYou()
    {
        return View();
    }
}