namespace NLBAudit.Core;

public interface IAuthorizationInfoProvider
{
    bool IsAuthenticated();
    
    string? GetUserName();
}