using MyWebApp.Application.DTOs.TimeTable;

namespace MyWebApp.ViewModels.TimeTable;

public class TrainScheduleVm
{
    public IReadOnlyList<TimeTableDto> Schedules { get; set; } = Array.Empty<TimeTableDto>();
}