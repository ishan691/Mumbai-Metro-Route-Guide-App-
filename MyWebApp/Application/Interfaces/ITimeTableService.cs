using MyWebApp.Application.DTOs.TimeTable;

namespace MyWebApp.Application.Interfaces;

public interface ITimeTableService
{
    Task<TrainScheduleResultDto> GetAllSchedulesAsync();
}