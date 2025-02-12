using NLBAudit.Core;

namespace NLBAudit.Store.EfCore;

internal static class MappingExtensions
{
    public static AuditLogEntity ToAuditLogEntity(this AuditInfo auditInfo)
    {
        return new AuditLogEntity
        {
            UserName = auditInfo.UserName,
            Path = auditInfo.Path,
            HttpMethod = auditInfo.HttpMethod,
            ServiceName = auditInfo.ServiceName,
            MethodName = auditInfo.MethodName,
            InputObj = auditInfo.InputObj,
            ReturnValue = auditInfo.ReturnValue,
            CreationTime = auditInfo.CreationTime,
            Duration = auditInfo.Duration,
            Exception = auditInfo.Exception?.ToString(),
            BrowserInfo = auditInfo.BrowserInfo,
            ClientIpAddress = auditInfo.ClientIpAddress,
        };
    }
    
    public static AuditInfo ToAuditInfo(this AuditLogEntity entity)
    {
        return new AuditInfo
        {
            UserName = entity.UserName,
            Path = entity.Path,
            HttpMethod = entity.HttpMethod,
            ServiceName = entity.ServiceName,
            MethodName = entity.MethodName,
            InputObj = entity.InputObj,
            ReturnValue = entity.ReturnValue,
            CreationTime = entity.CreationTime,
            Duration = entity.Duration,
            Exception = entity.Exception is not null ? new Exception(entity.Exception) : null,
            BrowserInfo = entity.BrowserInfo,
            ClientIpAddress = entity.ClientIpAddress,
        };
    }
}