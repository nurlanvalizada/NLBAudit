namespace NLBAudit.Store.EfCore;

public class AuditLogEntity
{
    public long Id { get; set; }
    
    public string? UserName { get; set; }
    
    public required string Path { get; set; }
    
    public required string HttpMethod { get; set; }
    
    public required string ServiceName { get; set; }
    
    public required string MethodName { get; set; }
    
    public string? InputObj { get; set; }
    
    public string? ReturnValue { get; set; }
    
    public DateTime CreationTime { get; set; }
    
    public int Duration { get; set; }
    
    public string? ClientIpAddress { get; set; }
    
    public string? BrowserInfo { get; set; }
    
    public string? Exception { get; set; }
}