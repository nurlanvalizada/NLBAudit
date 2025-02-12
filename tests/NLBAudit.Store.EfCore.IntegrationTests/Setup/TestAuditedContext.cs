using Microsoft.EntityFrameworkCore;
using NLBAudit.Store.EfCore.Extensions;

namespace NLBAudit.Store.EfCore.IntegrationTests.Setup;

public class TestAuditedContext(DbContextOptions<TestAuditedContext> options) : DbContext(options), IAuditedContext
{
    public DbSet<AuditLogEntity> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureEfCoreAuditing<int>();
        base.OnModelCreating(modelBuilder);
    }
}