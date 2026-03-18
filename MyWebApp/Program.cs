using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Application.Interfaces;
using MyWebApp.Application.Services;
using MyWebApp.Infrastructure.Data;
using MyWebApp.Infrastructure.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connStr = builder.Configuration.GetConnectionString("metro");

// Official Oracle provider
builder.Services.AddDbContext<SiteDbContext>(options => options.UseMySQL(connStr));

// cookie auth
builder
    .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/AdminLogin";
        options.AccessDeniedPath = "/Auth/AdminLogin";
    });

builder.Services.AddAuthorization();

// app services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStationService, StationService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<ITimeTableService, TimeTableService>();
builder.Services.AddScoped<IRouteService, RouteService>();

// password hashing
builder.Services.AddScoped<IPasswordHasher<Admin>, PasswordHasher<Admin>>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();
app.Run();

