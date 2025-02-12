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

        app.MapGet("/weatherforecast", (string testInfo) =>
           {
               var forecast = Enumerable.Range(1, 5).Select(index =>
                                            new WeatherForecast
                                            (
                                                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                                Random.Shared.Next(-20, 55),
                                                summaries[Random.Shared.Next(summaries.Length)],
                                                testInfo
                                            ))
                                        .ToArray();
               return forecast;
           })
           .AddEndpointFilter<MinimalApiEndpointAuditFilter>()
           .WithOpenApi();
        
        app.MapPost("/weatherforecast", (WeatherForecast weatherForecast) => weatherForecast)
           .AddEndpointFilter<MinimalApiEndpointAuditFilter>()
           .WithOpenApi();

        app.Run();
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary, string? TestInfo)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}