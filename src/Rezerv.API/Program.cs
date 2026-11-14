using Microsoft.EntityFrameworkCore;
using Rezerv.Application.Interfaces;
using Rezerv.Application.Services;
using Rezerv.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Rezerv.Infrastructure.Identity;
using Rezerv.Infrastructure.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Rezerv.Infrastructure.Settings;
using Rezerv.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()!;

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();

app.MapGet("/", () =>
{
    return "Welcome to Rezerv API!";
});

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "Healthy",
        application = "Rezerv API",
        timestamp = DateTime.UtcNow
    });
});

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    await dbContext.Database.MigrateAsync();
}

app.Run();