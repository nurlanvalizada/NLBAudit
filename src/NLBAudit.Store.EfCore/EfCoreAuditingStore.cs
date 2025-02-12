using Microsoft.EntityFrameworkCore;
using NLBAudit.Core;

namespace NLBAudit.Store.EfCore;

public class EfCoreAuditingStore(IAuditedContext context) : IAuditingStore
{
    public async Task SaveAsync(AuditInfo auditInfo, CancellationToken cancellationToken)
    {
        var entity = auditInfo.ToAuditLogEntity();
        await context.AuditLogs.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<long, AuditInfo>> FilterAsync(DateTime startDate, DateTime endDate, string? path, bool? hasException,
                                                                        int? minExecutionDuration, int? maxExecutionDuration, int skipCount, int maxResultCount,
                                                                        CancellationToken cancellationToken)
    {
        var query = context.AuditLogs
                           .Where(x => x.CreationTime >= startDate && x.CreationTime <= endDate)
                           .Where(x => path == null || x.Path == path)
                           .Where(x => hasException == null || x.Exception != null)
                           .Where(x => minExecutionDuration == null || x.Duration >= minExecutionDuration)
                           .Where(x => maxExecutionDuration == null || x.Duration <= maxExecutionDuration)
                           .Skip(skipCount)
                           .Take(maxResultCount)
                           .AsNoTracking();

        var items = await query.ToDictionaryAsync(x => x.Id, x => x.ToAuditInfo(), cancellationToken);
        return items;
    }
}