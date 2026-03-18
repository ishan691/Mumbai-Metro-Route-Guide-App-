namespace MyWebApp.Application.DTOs.Route;

public class JourneyPlannerResultDto
{
    public uint? SourceStationId { get; set; }
    public uint? DestinationStationId { get; set; }

    public string SourceStationName { get; set; } = string.Empty;
    public string DestinationStationName { get; set; } = string.Empty;

    public float Fare { get; set; }
    public TimeSpan Time { get; set; }
    public int Stop { get; set; }
    public int InterchangeStops { get; set; }

    public IReadOnlyList<RouteStationDto> Stations { get; set; } = Array.Empty<RouteStationDto>();
    public IReadOnlyList<StationOptionDto> StationOptions { get; set; } =
        Array.Empty<StationOptionDto>();
}