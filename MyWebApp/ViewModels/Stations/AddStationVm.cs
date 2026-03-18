using System.ComponentModel.DataAnnotations;

namespace MyWebApp.ViewModels.Stations;

public class AddStationVm
{
    [Required]
    public uint StationId { get; set; }

    [Required]
    public string StationName { get; set; } = string.Empty;

    [Required]
    public uint RouteId { get; set; }

    public uint? PreviousStationId { get; set; }
    public uint? NextStationId { get; set; }
}