namespace MyWebApp.Application.DTOs.TimeTable;

public class TimeTableDto
{
    public uint Id { get; set; }
    public uint StationIdFrom { get; set; }
    public string StationNameFrom { get; set; } = string.Empty;
    public uint StationIdTo { get; set; }
    public string StationNameTo { get; set; } = string.Empty;
    public TimeSpan FirstTrain { get; set; }
    public TimeSpan LastTrain { get; set; }
}