using NLBAudit.Core;

namespace NLBAudit.AspNetCore;

public class AspNetCoreAuthorizationInfoProvider(IHttpContextAccessor httpContextAccessor) : IAuthorizationInfoProvider
{
    public bool IsAuthenticated()
    {
        var isAuthenticated = httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        return isAuthenticated;
    }

    public string? GetUserName()
    {
        string? userId = null;
        var subClaim = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type is "username" or "email");

        if(subClaim != null)
        {
            userId = subClaim.Value;
        }

        return userId;
    }
}