namespace NLBAudit.Core;

public interface IAuditingStore<TUserId>
{
    Task SaveAsync(AuditInfo<TUserId> auditInfo, CancellationToken cancellationToken);
}