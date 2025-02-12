using Microsoft.EntityFrameworkCore;

namespace NLBAudit.Store.EfCore;

public interface IAuditedContext
{
    public DbSet<AuditLogEntity> AuditLogs { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}