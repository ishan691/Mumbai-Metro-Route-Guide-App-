using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.Application.Interfaces;
using MyWebApp.ViewModels.TimeTable;

namespace MyWebApp.Controllers;

[AllowAnonymous]
public class TimeTableController : Controller
{
    private readonly ITimeTableService _timeTableService;

    public TimeTableController(ITimeTableService timeTableService)
    {
        _timeTableService = timeTableService;
    }

    [HttpGet]
    public async Task<IActionResult> TrainSchedule()
    {
        var result = await _timeTableService.GetAllSchedulesAsync();

        var vm = new TrainScheduleVm { Schedules = result.Items };

        return View(vm);
    }
}
