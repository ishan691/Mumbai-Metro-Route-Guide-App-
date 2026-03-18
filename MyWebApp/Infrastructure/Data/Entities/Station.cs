namespace MyWebApp.Infrastructure.Data.Entities;

public class Station
{
    public uint StationId { get; set; }
    public string StationName { get; set; } = string.Empty;

    public uint RouteId { get; set; }

    public uint? PreviousStationId { get; set; }
    public uint? NextStationId { get; set; }
}
