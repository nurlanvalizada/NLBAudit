namespace NLBAudit.Core;

public class AuditInfo<TUserId>
{
    public TUserId? UserId { get; set; }
    
    public required string Path { get; set; }
    
    public required string HttpMethod { get; set; }
    
    public required string ServiceName { get; set; }
    
    public required string MethodName { get; set; }
    
    public string? InputObj { get; set; }
    
    public string? ReturnValue { get; set; }
    
    public DateTime CreationTime { get; set; }
    
    /// <summary>
    /// Duration of the method call in milliseconds.
    /// </summary>
    public int Duration { get; set; }
    
    public string? ClientIpAddress { get; set; }
    
    public string? BrowserInfo { get; set; }
    
    public Exception? Exception { get; set; }

    public override string ToString()
    {
        var loggedUserId = UserId is not null
            ? "user: " + UserId
            : "an anonymous user";

        var exceptionOrSuccessMessage = Exception != null
            ? "exception: " + Exception.Message
            : "success";

        return $"AUDIT INFO: {HttpMethod} request to {Path} handled using {ServiceName}.{MethodName} is executed by {loggedUserId} in {Duration} ms from " +
               $"{ClientIpAddress} IP address with {exceptionOrSuccessMessage}.";
    }
}