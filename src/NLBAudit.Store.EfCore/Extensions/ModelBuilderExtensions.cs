using Microsoft.EntityFrameworkCore;

namespace NLBAudit.Store.EfCore.Extensions;

public static class EfCoreModelBuilderExtensions
{
    public static void ConfigureEfCoreAuditing<TUserId>(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLogEntity<TUserId>>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.ServiceName).HasMaxLength(256).IsRequired();
            b.Property(x => x.MethodName).HasMaxLength(256).IsRequired();
            b.Property(x => x.InputObj).HasMaxLength(4000);
            b.Property(x => x.ReturnValue).HasMaxLength(4000);
            b.Property(x => x.ClientIpAddress).HasMaxLength(64);
            b.Property(x => x.BrowserInfo).HasMaxLength(512);
            b.Property(x => x.CustomData).HasMaxLength(2000);
        });
    }
}