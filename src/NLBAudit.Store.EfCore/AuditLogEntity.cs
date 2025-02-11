namespace NLBAudit.Store.EfCore;

public class AuditLogEntity<TUserId>
{
    public long Id { get; set; }
    
    public TUserId? UserId { get; set; }
    
    public required string Path { get; set; }
    
    public required string HttpMethod { get; set; }
    
    public required string ServiceName { get; set; }
    
    public string MethodName { get; set; }
    
    public string? InputObj { get; set; }
    
    public string? ReturnValue { get; set; }
    
    public DateTime CreationTime { get; set; }
    
    public int Duration { get; set; }
    
    public string? ClientIpAddress { get; set; }
    
    public string? BrowserInfo { get; set; }
    
    public string? Exception { get; set; }
}