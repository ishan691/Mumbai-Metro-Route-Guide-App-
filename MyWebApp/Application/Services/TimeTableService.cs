using Microsoft.EntityFrameworkCore;
using MyWebApp.Application.DTOs.TimeTable;
using MyWebApp.Application.Interfaces;
using MyWebApp.Infrastructure.Data;

namespace MyWebApp.Application.Services;

public class TimeTableService : ITimeTableService
{
    private readonly SiteDbContext _db;

    public TimeTableService(SiteDbContext db)
    {
        _db = db;
    }

    public async Task<TrainScheduleResultDto> GetAllSchedulesAsync()
    {
        var stations = await _db
            .Stations.AsNoTracking()
            .ToDictionaryAsync(x => x.StationId, x => x.StationName);

        var items = await _db
            .TimeTables.AsNoTracking()
            .OrderBy(x => x.StationIdFrom)
            .ThenBy(x => x.StationIdTo)
            .Select(x => new TimeTableDto
            {
                Id = x.Id,
                StationIdFrom = x.StationIdFrom,
                StationIdTo = x.StationIdTo,
                FirstTrain = x.FirstTrain,
                LastTrain = x.LastTrain,
            })
            .ToListAsync();

        foreach (var item in items)
        {
            item.StationNameFrom = stations.TryGetValue(item.StationIdFrom, out var fromName)
                ? fromName
                : string.Empty;

            item.StationNameTo = stations.TryGetValue(item.StationIdTo, out var toName)
                ? toName
                : string.Empty;
        }

        return new TrainScheduleResultDto { Items = items };
    }
}
