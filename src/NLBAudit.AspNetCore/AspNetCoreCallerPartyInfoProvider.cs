using System.Net;
using NLBAudit.Core;

namespace NLBAudit.AspNetCore;

public class AspNetCoreCallerPartyInfoProvider(IHttpContextAccessor httpContextAccessor, ILogger<AspNetCoreCallerPartyInfoProvider> logger) : ICallerPartyInfoProvider
{
    public string? BrowserInfo => GetBrowserInfo();

    public string? ClientIpAddress => GetClientIpAddress();
    

    private string? GetBrowserInfo()
    {
        var httpContext = httpContextAccessor.HttpContext;
        return httpContext?.Request.Headers.UserAgent;
    }

    private string? GetClientIpAddress()
    {
        try
        {
            var httpContext = httpContextAccessor.HttpContext;
            string? ip = null;

            // X-Forwarded-For: client, proxy1, proxy2
            if (httpContext?.Request.Headers != null && httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                var forwardedForValue = forwardedFor.FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedForValue))
                {
                    ip = forwardedForValue.Split(',')[0].Trim();
                }
            }
            
            if (string.IsNullOrEmpty(ip) &&
                httpContext?.Connection.RemoteIpAddress != null)
            {
                ip = httpContext.Connection.RemoteIpAddress.ToString();
            }
            
            if (!string.IsNullOrEmpty(ip) && IPAddress.TryParse(ip, out _))
            {
                return ip;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to get ip address");
        }
            
        return null;
    }
}