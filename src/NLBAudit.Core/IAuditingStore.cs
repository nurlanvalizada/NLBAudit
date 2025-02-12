namespace NLBAudit.Core;

public interface IAuditingStore
{
    Task SaveAsync(AuditInfo auditInfo, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, AuditInfo>> FilterAsync(DateTime startDate, DateTime endDate, string? path, bool? hasException,
                                                           int? minExecutionDuration, int? maxExecutionDuration, int skipCount, int maxResultCount,
                                                           CancellationToken cancellationToken);
}