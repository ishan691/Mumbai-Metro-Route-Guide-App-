using Microsoft.EntityFrameworkCore;
using MyWebApp.Application.DTOs.Route;
using MyWebApp.Application.Interfaces;
using MyWebApp.Infrastructure.Data;


namespace MyWebApp.Application.Services;

public class RouteService : IRouteService
{
    private readonly SiteDbContext _db;

    public RouteService(SiteDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<StationOptionDto>> GetStationOptionsAsync()
    {
        return await _db
            .Stations.AsNoTracking()
            .OrderBy(x => x.StationName)
            .Select(x => new StationOptionDto
            {
                StationId = x.StationId,
                StationName = x.StationName,
            })
            .ToListAsync();
    }

    public async Task<JourneyPlannerResultDto> GetJourneyAsync(
        uint? sourceStationId,
        uint? destinationStationId
    )
    {
        var stationOptions = await GetStationOptionsAsync();

        var result = new JourneyPlannerResultDto
        {
            SourceStationId = sourceStationId,
            DestinationStationId = destinationStationId,
            StationOptions = stationOptions,
        };

        if (
            !sourceStationId.HasValue
            || !destinationStationId.HasValue
            || sourceStationId.Value == 0
            || destinationStationId.Value == 0
        )
        {
            return result;
        }

        if (sourceStationId.Value == destinationStationId.Value)
        {
            var sameStation = await _db
                .Stations.AsNoTracking()
                .Where(x => x.StationId == sourceStationId.Value)
                .Select(x => new RouteStationDto
                {
                    StationId = x.StationId,
                    StationName = x.StationName,
                    RouteId = x.RouteId,
                })
                .FirstOrDefaultAsync();

            if (sameStation != null)
            {
                result.SourceStationName = sameStation.StationName;
                result.DestinationStationName = sameStation.StationName;
                result.Stations = new List<RouteStationDto> { sameStation };
            }

            result.Fare = 0;
            result.Time = TimeSpan.Zero;
            result.Stop = 0;
            result.InterchangeStops = 0;

            return result;
        }

        var sourceStation = await _db
            .Stations.AsNoTracking()
            .FirstOrDefaultAsync(x => x.StationId == sourceStationId.Value);

        var destinationStation = await _db
            .Stations.AsNoTracking()
            .FirstOrDefaultAsync(x => x.StationId == destinationStationId.Value);

        if (sourceStation == null || destinationStation == null)
        {
            return result;
        }

        result.SourceStationName = sourceStation.StationName;
        result.DestinationStationName = destinationStation.StationName;

        var directSummary = await _db
            .RouteStations.AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.StationIdFrom == sourceStationId.Value
                && x.StationIdTo == destinationStationId.Value
            );

        if (directSummary != null)
        {
            result.Fare = directSummary.Fare;
            result.Time = directSummary.Time;
            result.Stop = directSummary.Stop;
            result.InterchangeStops = directSummary.InterchangeStops;
        }
        else
        {
            result.Fare = 0;
            result.Time = TimeSpan.Zero;
            result.Stop = 0;
            result.InterchangeStops = 0;
        }

        var stations = await BuildPathAsync(sourceStationId.Value, destinationStationId.Value);
        result.Stations = stations;

        return result;
    }

    private async Task<IReadOnlyList<RouteStationDto>> BuildPathAsync(
        uint sourceStationId,
        uint destinationStationId
    )
    {
        var allStations = await _db
            .Stations.AsNoTracking()
            .Select(x => new
            {
                x.StationId,
                x.StationName,
                x.RouteId,
                x.PreviousStationId,
                x.NextStationId,
            })
            .ToListAsync();

        var stationMap = allStations.ToDictionary(x => x.StationId);

        var graph = new Dictionary<uint, List<uint>>();

        foreach (var station in allStations)
        {
            if (!graph.ContainsKey(station.StationId))
            {
                graph[station.StationId] = new List<uint>();
            }

            if (
                station.PreviousStationId.HasValue
                && stationMap.ContainsKey(station.PreviousStationId.Value)
            )
            {
                graph[station.StationId].Add(station.PreviousStationId.Value);

                if (!graph.ContainsKey(station.PreviousStationId.Value))
                    graph[station.PreviousStationId.Value] = new List<uint>();

                if (!graph[station.PreviousStationId.Value].Contains(station.StationId))
                    graph[station.PreviousStationId.Value].Add(station.StationId);
            }

            if (
                station.NextStationId.HasValue
                && stationMap.ContainsKey(station.NextStationId.Value)
            )
            {
                graph[station.StationId].Add(station.NextStationId.Value);

                if (!graph.ContainsKey(station.NextStationId.Value))
                    graph[station.NextStationId.Value] = new List<uint>();

                if (!graph[station.NextStationId.Value].Contains(station.StationId))
                    graph[station.NextStationId.Value].Add(station.StationId);
            }
        }

        if (!graph.ContainsKey(sourceStationId) || !graph.ContainsKey(destinationStationId))
        {
            return Array.Empty<RouteStationDto>();
        }

        var queue = new Queue<uint>();
        var visited = new HashSet<uint>();
        var parent = new Dictionary<uint, uint?>();

        queue.Enqueue(sourceStationId);
        visited.Add(sourceStationId);
        parent[sourceStationId] = null;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current == destinationStationId)
                break;

            foreach (var next in graph[current].Distinct())
            {
                if (visited.Contains(next))
                    continue;

                visited.Add(next);
                parent[next] = current;
                queue.Enqueue(next);
            }
        }

        if (!visited.Contains(destinationStationId))
        {
            return Array.Empty<RouteStationDto>();
        }

        var path = new List<uint>();
        uint? cursor = destinationStationId;

        while (cursor.HasValue)
        {
            path.Add(cursor.Value);
            cursor = parent[cursor.Value];
        }

        path.Reverse();

        return path.Where(stationMap.ContainsKey)
            .Select(id => new RouteStationDto
            {
                StationId = stationMap[id].StationId,
                StationName = stationMap[id].StationName,
                RouteId = stationMap[id].RouteId,
            })
            .ToList();
    }
}
