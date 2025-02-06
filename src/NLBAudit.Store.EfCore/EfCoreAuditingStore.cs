using NLBAudit.Core;

namespace NLBAudit.Store.EfCore;

public class EfCoreAuditingStore<TUserId>(IAuditedContext<TUserId> context) : IAuditingStore<TUserId>
{
    public async Task SaveAsync(AuditInfo<TUserId> auditInfo, CancellationToken cancellationToken)
    {
        var entity = auditInfo.ToAuditLogEntity();
        await context.AuditLogs.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}