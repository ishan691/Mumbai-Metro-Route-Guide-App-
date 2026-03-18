using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Infrastructure.Data.Entities;

public class TimeTable
{
    public uint Id { get; set; }

    [Required]
    public uint StationIdFrom { get; set; }

    [Required]
    public uint StationIdTo { get; set; }

    [Required]
    public TimeSpan FirstTrain { get; set; }

    [Required]
    public TimeSpan LastTrain { get; set; }
}