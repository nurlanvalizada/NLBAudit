using NLBAudit.Core;

namespace NLBAudit.Store.EfCore;

internal static class MappingExtensions
{
    public static AuditLogEntity<TUserId> ToAuditLogEntity<TUserId>(this AuditInfo<TUserId> auditInfo)
    {
        return new AuditLogEntity<TUserId>
        {
            UserId = auditInfo.UserId,
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
            ClientIpAddress = auditInfo.ClientIpAddress
        };
    }
}