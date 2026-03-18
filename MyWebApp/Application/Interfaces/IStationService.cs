using MyWebApp.Application.DTOs.Stations;

namespace MyWebApp.Application.Interfaces;

public interface IStationService
{
    Task<List<StationDto>> GetAllStationsAsync();
    Task<(bool Success, string? ErrorMessage)> AddStationAsync(StationCreateRequestDto request);
    Task<LineStationsResultDto> GetStationsByLinesAsync();
}