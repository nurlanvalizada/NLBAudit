using NLBAudit.AspNetCore.MinimalApis;

namespace NLBAudit.AspNetCore.Tests.Setup;

public static class TestMinimalApis
{
    public static void ConfigureTestMinimalApi(this IEndpointRouteBuilder app)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing"
        };

        app.MapGet("/weatherforecast", (string testInfo) =>
           {
               var forecast = Enumerable.Range(1, 2).Select(index =>
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
           .AddEndpointFilter<MinimalApiEndpointAuditFilter<int>>();
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary, string? TestInfo)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}