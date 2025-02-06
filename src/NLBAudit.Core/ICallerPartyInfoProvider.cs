namespace NLBAudit.Core;

public interface ICallerPartyInfoProvider
{
    string? BrowserInfo { get; }
    string? ClientIpAddress { get; }
}