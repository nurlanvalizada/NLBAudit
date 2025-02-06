using Microsoft.EntityFrameworkCore;
using NLBAudit.Store.EfCore;
using NLBAudit.Store.EfCore.Extensions;

namespace NLBAudit.Sample;

public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options), IAuditedContext<int>
{
    public DbSet<AuditLogEntity<int>> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureEfCoreAuditing<int>();
        modelBuilder.Entity<AuditLogEntity<int>>().ToTable("AuditLogs");
    }
}