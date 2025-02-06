using Microsoft.EntityFrameworkCore;

namespace NLBAudit.Store.EfCore;

public interface IAuditedContext<TUserId>
{
    public DbSet<AuditLogEntity<TUserId>> AuditLogs { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}