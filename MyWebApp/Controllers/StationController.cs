using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.Application.DTOs.Stations;
using MyWebApp.Application.Interfaces;
using MyWebApp.ViewModels.Stations;

namespace MyWebApp.Controllers;

public class StationController : Controller
{
    private readonly IStationService _stationService;

    public StationController(IStationService stationService)
    {
        _stationService = stationService;
    }

    [HttpGet]
    public async Task<IActionResult> ViewStation()
    {
        var stations = await _stationService.GetAllStationsAsync();

        var vm = stations
            .Select(s => new StationListItemVm
            {
                StationId = s.StationId,
                StationName = s.StationName,
                RouteId = s.RouteId,
                PreviousStationId = s.PreviousStationId,
                NextStationId = s.NextStationId,
            })
            .ToList();

        return View(vm);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult AddStation()
    {
        return View(new AddStationVm());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddStation(AddStationVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var result = await _stationService.AddStationAsync(
            new StationCreateRequestDto
            {
                StationId = vm.StationId,
                StationName = vm.StationName,
                RouteId = vm.RouteId,
                PreviousStationId = vm.PreviousStationId,
                NextStationId = vm.NextStationId,
            }
        );

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Failed to add station.");
            return View(vm);
        }

        return RedirectToAction(nameof(ViewStation));
    }

    [HttpGet]
    public async Task<IActionResult> DisplayAllLineStations()
    {
        var result = await _stationService.GetStationsByLinesAsync();

        var vm = new LineStationsVm
        {
            YellowLineStations = result.YellowLineStations,
            RedLineStations = result.RedLineStations,
            BlueLineStations = result.BlueLineStations,
        };

        return View(vm);
    }
}
