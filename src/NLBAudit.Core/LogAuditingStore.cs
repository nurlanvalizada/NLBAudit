using Microsoft.Extensions.Logging;

namespace NLBAudit.Core;

internal class LogAuditingStore(ILogger<LogAuditingStore> logger) : IAuditingStore
{
    public AuditInfo? LastAuditInfo { get; private set; }

    public Task SaveAsync(AuditInfo auditInfo, CancellationToken cancellationToken)
    {
        if(auditInfo.Exception == null)
        {
            logger.LogInformation(auditInfo.ToString());
        }
        else
        {
            logger.LogWarning(auditInfo.ToString());
        }

        LastAuditInfo = auditInfo;
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyDictionary<long, AuditInfo>> FilterAsync(DateTime startDate, DateTime endDate, string? path, bool? hasException,
                                                                        int? minExecutionDuration, int? maxExecutionDuration, int skipCount, int maxResultCount,
                                                                        CancellationToken cancellationToken)
    {
        await Task.Delay(100, cancellationToken);
        return new Dictionary<long, AuditInfo>();
    }
}