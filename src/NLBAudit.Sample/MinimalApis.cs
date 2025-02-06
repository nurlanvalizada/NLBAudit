using NLBAudit.AspNetCore.MinimalApis;

namespace NLBAudit.Sample;

public static class MinimalApis
{
    public static void Configure(WebApplication app)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/weatherforecast", () =>
           {
               var forecast = Enumerable.Range(1, 5).Select(index =>
                                            new WeatherForecast
                                            (
                                                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                                Random.Shared.Next(-20, 55),
                                                summaries[Random.Shared.Next(summaries.Length)]
                                            ))
                                        .ToArray();
               return forecast;
           })
           .AddEndpointFilter<MinimalApiEndpointAuditFilter<int>>()
           .WithName("GetWeatherForecast")
           .WithOpenApi();

        app.Run();
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}