namespace MyWebApp.ViewModels.Stations;

public class StationListItemVm
{
    public uint StationId { get; set; }
    public string StationName { get; set; } = string.Empty;
    public uint RouteId { get; set; }
    public uint? PreviousStationId { get; set; }
    public uint? NextStationId { get; set; }
}