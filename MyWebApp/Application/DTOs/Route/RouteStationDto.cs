namespace MyWebApp.Application.DTOs.Route;

public class RouteStationDto
{
    public uint StationId { get; set; }
    public string StationName { get; set; } = string.Empty;
    public uint RouteId { get; set; }
}