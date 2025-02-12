using NLBAudit.Core;

namespace NLBAudit.AspNetCore.Controllers.Dto;

public record AuditLogListDto(
    long Id,
    string? UserName,
    string Path,
    string HttpMethod,
    string ServiceName,
    string MethodName,
    string? InputObj,
    DateTime CreationTime,
    int Duration,
    string? ClientIpAddress,
    string? BrowserInfo,
    string? Exception)
{
    public static AuditLogListDto FromAuditLogInfo(AuditInfo auditInfo, long id)
    {
        return new AuditLogListDto(
            id,
            auditInfo.UserName,
            auditInfo.Path,
            auditInfo.HttpMethod,
            auditInfo.ServiceName,
            auditInfo.MethodName,
            auditInfo.InputObj,
            auditInfo.CreationTime,
            auditInfo.Duration,
            auditInfo.ClientIpAddress,
            auditInfo.BrowserInfo,
            auditInfo.Exception?.Message
        );
    }
}