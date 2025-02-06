using Microsoft.Extensions.Logging;

namespace NLBAudit.Core;

public class LogAuditingStore<TUserId>(ILogger<LogAuditingStore<TUserId>> logger) : IAuditingStore<TUserId>
{
    public Task SaveAsync(AuditInfo<TUserId> auditInfo, CancellationToken cancellationToken)
    {
        if (auditInfo.Exception == null)
        {
            logger.LogInformation(auditInfo.ToString());
        }
        else
        {
            logger.LogError(auditInfo.ToString());
        }

        return Task.CompletedTask;
    }
}