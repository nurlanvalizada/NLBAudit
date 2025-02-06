namespace NLBAudit.Core;

internal class TestCallerPartyInfoProvider : ICallerPartyInfoProvider
{
    public string? BrowserInfo => "Test Browser, Version 1.0";
    public string? ClientIpAddress => "0.0.0.0";
}