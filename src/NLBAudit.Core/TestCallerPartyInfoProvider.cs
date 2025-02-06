namespace NLBAudit.Core;

public class TestCallerPartyInfoProvider : ICallerPartyInfoProvider
{
    public string? BrowserInfo { get; } = "Test Browser, Version 1.0";
    public string? ClientIpAddress { get; } = "0.0.0.0";
}