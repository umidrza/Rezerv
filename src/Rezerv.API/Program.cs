var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

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

app.Run();