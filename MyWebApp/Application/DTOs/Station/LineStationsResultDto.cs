namespace MyWebApp.Application.DTOs.Stations;

public class LineStationsResultDto
{
    public List<string> YellowLineStations { get; set; } = [];
    public List<string> RedLineStations { get; set; } = [];
    public List<string> BlueLineStations { get; set; } = [];
}
