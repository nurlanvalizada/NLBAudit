using NLBAudit.Core;

namespace NLBAudit.AspNetCore;

public class AspNetCoreAuthorizationInfoProvider<TUserId>(IHttpContextAccessor httpContextAccessor)
    : IAuthorizationInfoProvider<TUserId>
{
    public bool IsAuthenticated()
    {
        var isAuthenticated = httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        return isAuthenticated;
    }

    public TUserId? GetUserId()
    {
        TUserId? userId = default;
        var subClaim = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "sub");

        if(subClaim != null)
        {
            userId = (TUserId)Convert.ChangeType(subClaim.Value, typeof(TUserId));
        }
        
        return userId;
    }
}