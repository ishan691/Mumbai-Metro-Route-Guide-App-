namespace MyWebApp.Application.DTOs.TimeTable;

public class TrainScheduleResultDto
{
    public IReadOnlyList<TimeTableDto> Items { get; set; } = Array.Empty<TimeTableDto>();
}