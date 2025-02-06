namespace NLBAudit.Core;

public class AuditInfo<TUserId>
{
    public TUserId? UserId { get; set; }
    
    public string ServiceName { get; set; }
    
    public string MethodName { get; set; }
    
    public string? InputObj { get; set; }
    
    public string? ReturnValue { get; set; }
    
    public DateTime CreationTime { get; set; }
    
    public int Duration { get; set; }
    
    public string? ClientIpAddress { get; set; }
    
    public string? BrowserInfo { get; set; }
    
    public string? CustomData { get; set; }
    
    public Exception? Exception { get; set; }

    public override string ToString()
    {
        var loggedUserId = UserId is not null
            ? "user: " + UserId
            : "an anonymous user";

        var exceptionOrSuccessMessage = Exception != null
            ? "exception: " + Exception.Message
            : "success";

        return $"AUDIT LOG: {ServiceName}.{MethodName} is executed by {loggedUserId} in {Duration} ms from {ClientIpAddress} IP address with {exceptionOrSuccessMessage}.";
    }
}