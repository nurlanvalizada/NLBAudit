namespace NLBAudit.Core;

internal class TestAuthorizationInfoProvider : IAuthorizationInfoProvider
{
    public bool IsAuthenticated()
    {
        return true;
    }

    public string? GetUserName()
    {
        return "test";
    }
}