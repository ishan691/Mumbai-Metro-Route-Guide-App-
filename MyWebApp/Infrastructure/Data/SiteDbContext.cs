using Microsoft.EntityFrameworkCore;
using MyWebApp.Infrastructure.Data.Entities;

namespace MyWebApp.Infrastructure.Data;

public class SiteDbContext : DbContext
{
    public SiteDbContext(DbContextOptions<SiteDbContext> options)
        : base(options) { }

    public DbSet<Admin> Admins { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Station> Stations { get; set; }

    public DbSet<RouteLine> RouteLines { get; set; }

    public DbSet<RouteStation> RouteStations { get; set; }

    public DbSet<TimeTable> TimeTables { get; set; }

    public DbSet<Feedback> Feedbacks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("admin");
            entity.HasKey(x => x.Username);
            entity.Property(x => x.Username).HasColumnName("username");
            entity.Property(x => x.PasswordHash).HasColumnName("password").HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(x => x.Username);
            entity.Property(x => x.Username).HasColumnName("username");
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(200);
            entity.Property(x => x.Email).HasColumnName("email").HasMaxLength(255);
            entity.Property(x => x.PasswordHash).HasColumnName("password").HasMaxLength(255);
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.ToTable("stations");
            entity.HasKey(x => x.StationId);
            entity.Property(x => x.StationId).HasColumnName("station_id");
            entity.Property(x => x.StationName).HasColumnName("station_name").HasMaxLength(200);
            entity.Property(x => x.RouteId).HasColumnName("route_id");
            entity.Property(x => x.PreviousStationId).HasColumnName("previous_station_id");
            entity.Property(x => x.NextStationId).HasColumnName("next_station_id");
        });

        modelBuilder.Entity<RouteLine>(entity =>
        {
            entity.ToTable("route_lines");
            entity.HasKey(x => x.RouteId);
            entity.Property(x => x.RouteId).HasColumnName("route_id");
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<RouteStation>(entity =>
        {
            entity.ToTable("route_stations");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.StationIdFrom).HasColumnName("station_id_from");
            entity.Property(x => x.StationIdTo).HasColumnName("station_id_to");
            entity.Property(x => x.Stop).HasColumnName("stop");
            entity.Property(x => x.Time).HasColumnName("time");
            entity.Property(x => x.Fare).HasColumnName("fare");
            entity.Property(x => x.InterchangeStops).HasColumnName("interchange_stops");

            entity.HasIndex(x => x.StationIdFrom);
            entity.HasIndex(x => x.StationIdTo);
        });

        modelBuilder.Entity<TimeTable>(entity =>
        {
            entity.ToTable("time_table");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.StationIdFrom).HasColumnName("station_id_from").IsRequired();
            entity.Property(x => x.StationIdTo).HasColumnName("station_id_to").IsRequired();
            entity.Property(x => x.FirstTrain).HasColumnName("first_train").IsRequired();
            entity.Property(x => x.LastTrain).HasColumnName("last_train").IsRequired();

            entity.HasIndex(x => x.StationIdFrom);
            entity.HasIndex(x => x.StationIdTo);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.ToTable("feedbacks");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedOnAdd();
            entity.Property(x => x.EmailId).HasColumnName("EmailId").HasMaxLength(255).IsRequired();
            entity.Property(x => x.OverallRating).HasColumnName("OverallRating");
            entity.Property(x => x.CleanlinessRating).HasColumnName("CleanlinessRating");
            entity.Property(x => x.FacilitiesRating).HasColumnName("FacilitiesRating");
            entity.Property(x => x.AccessibilityRating).HasColumnName("AccessibilityRating");
            entity.Property(x => x.Suggestions)
                .HasColumnName("Suggestions")
                .HasMaxLength(500)
                .IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("CreatedAt").IsRequired();

            entity.HasIndex(x => x.EmailId);
            entity.HasIndex(x => x.CreatedAt);
        });
    }
}