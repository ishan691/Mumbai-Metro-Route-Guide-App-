using Microsoft.EntityFrameworkCore;
using MyWebApp.Application.DTOs.Stations;
using MyWebApp.Application.Interfaces;
using MyWebApp.Infrastructure.Data;
using MyWebApp.Infrastructure.Data.Entities;

namespace MyWebApp.Application.Services;

public class StationService : IStationService
{
    private readonly SiteDbContext _db;

    public StationService(SiteDbContext db)
    {
        _db = db;
    }

    public async Task<List<StationDto>> GetAllStationsAsync()
    {
        return await _db
            .Stations.AsNoTracking()
            .OrderBy(s => s.RouteId)
            .ThenBy(s => s.StationId)
            .Select(s => new StationDto
            {
                StationId = s.StationId,
                StationName = s.StationName,
                RouteId = s.RouteId,
                PreviousStationId = s.PreviousStationId,
                NextStationId = s.NextStationId,
            })
            .ToListAsync();
    }

    public async Task<(bool Success, string? ErrorMessage)> AddStationAsync(
        StationCreateRequestDto request
    )
    {
        // Duplicate PK check (important interview topic)
        bool exists = await _db.Stations.AnyAsync(s => s.StationId == request.StationId);
        if (exists)
            return (false, "StationId already exists.");

        var entity = new Station
        {
            StationId = request.StationId,
            StationName = request.StationName.Trim(),
            RouteId = request.RouteId,
            PreviousStationId = request.PreviousStationId,
            NextStationId = request.NextStationId,
        };

        _db.Stations.Add(entity);
        await _db.SaveChangesAsync();
        return (true, null);
    }

    public async Task<LineStationsResultDto> GetStationsByLinesAsync()
    {
        // 1 query per line; readable + enough for small project
        var yellow = await _db
            .Stations.AsNoTracking()
            .Where(s => s.RouteId == 1)
            .OrderBy(s => s.StationId)
            .Select(s => s.StationName)
            .ToListAsync();

        var red = await _db
            .Stations.AsNoTracking()
            .Where(s => s.RouteId == 2)
            .OrderBy(s => s.StationId)
            .Select(s => s.StationName)
            .ToListAsync();

        var blue = await _db
            .Stations.AsNoTracking()
            .Where(s => s.RouteId == 3)
            .OrderBy(s => s.StationId)
            .Select(s => s.StationName)
            .ToListAsync();

        return new LineStationsResultDto
        {
            YellowLineStations = yellow,
            RedLineStations = red,
            BlueLineStations = blue,
        };
    }
}
