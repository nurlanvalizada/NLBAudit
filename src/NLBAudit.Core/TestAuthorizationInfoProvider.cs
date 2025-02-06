namespace NLBAudit.Core;

public class TestAuthorizationInfoProvider<TUserId> : IAuthorizationInfoProvider<TUserId>
{
    public bool IsAuthenticated()
    {
        return true;
    }

    public TUserId? GetUserId()
    {
        if(typeof(TUserId) == typeof(int))
        {
            return (TUserId)(object)1;
        }

        return default;
    }
}