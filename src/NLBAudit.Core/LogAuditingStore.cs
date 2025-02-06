using Microsoft.Extensions.Logging;

namespace NLBAudit.Core;

internal class LogAuditingStore<TUserId>(ILogger<LogAuditingStore<TUserId>> logger) : IAuditingStore<TUserId>
{
    public Task SaveAsync(AuditInfo<TUserId> auditInfo, CancellationToken cancellationToken)
    {
        if (auditInfo.Exception == null)
        {
            logger.LogInformation(auditInfo.ToString());
        }
        else
        {
            logger.LogWarning(auditInfo.ToString());
        }

        return Task.CompletedTask;
    }
}