namespace NLBAudit.Core;

public interface IAuthorizationInfoProvider<out TUserId>
{
    bool IsAuthenticated();
    
    TUserId? GetUserId();
}