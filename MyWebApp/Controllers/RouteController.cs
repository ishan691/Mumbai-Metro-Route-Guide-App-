using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.Application.Interfaces;
using MyWebApp.ViewModels.Route;

namespace MyWebApp.Controllers;

[AllowAnonymous]
public class RouteController : Controller
{
    private readonly IRouteService _routeService;

    public RouteController(IRouteService routeService)
    {
        _routeService = routeService;
    }

    [HttpGet]
    public async Task<IActionResult> JourneyPlanner(uint? sourceStationId, uint? destinationStationId)
    {
        var result = await _routeService.GetJourneyAsync(sourceStationId, destinationStationId);

        var vm = new JourneyPlannerVm
        {
            SourceStationId = result.SourceStationId,
            DestinationStationId = result.DestinationStationId,
            Fare = result.Fare,
            Time = result.Time,
            Stop = result.Stop,
            InterchangeStops = result.InterchangeStops,
            Stations = result.Stations,
            StationOptions = result.StationOptions
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> MetroBetweenStations(
        uint? sourceStationId,
        uint? destinationStationId
    )
    {
        var result = await _routeService.GetJourneyAsync(sourceStationId, destinationStationId);

        var vm = new JourneyPlannerVm
        {
            SourceStationId = result.SourceStationId,
            DestinationStationId = result.DestinationStationId,
            Fare = result.Fare,
            Time = result.Time,
            Stop = result.Stop,
            InterchangeStops = result.InterchangeStops,
            Stations = result.Stations,
            StationOptions = result.StationOptions
        };

        return View(vm);
    }
}