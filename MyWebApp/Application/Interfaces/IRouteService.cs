using MyWebApp.Application.DTOs.Route;

namespace MyWebApp.Application.Interfaces;

public interface IRouteService
{
    Task<IReadOnlyList<StationOptionDto>> GetStationOptionsAsync();
    Task<JourneyPlannerResultDto> GetJourneyAsync(
        uint? sourceStationId,
        uint? destinationStationId
    );
}
