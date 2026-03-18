using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Infrastructure.Data.Entities;

public class RouteStation
{
    public uint Id { get; set; }

    [Required]
    public uint StationIdFrom { get; set; }

    [Required]
    public uint StationIdTo { get; set; }

    public int Stop { get; set; }

    public TimeSpan Time { get; set; }

    public float Fare { get; set; }

    public int InterchangeStops { get; set; }
}