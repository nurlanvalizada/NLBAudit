using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NLBAudit.Core;

namespace NLBAudit.Store.EfCore.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureAuditingEfCoreStore<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
    {
        services.Replace(ServiceDescriptor.Scoped<IAuditingStore, EfCoreAuditingStore>());
        services.AddScoped<IAuditedContext>(provider =>
        {
            var service = provider.GetService<TDbContext>();
            if(service is IAuditedContext adc)
            {
                return adc;
            }

            throw new InvalidOperationException($"The DbContext {typeof(TDbContext).Name} must implement {nameof(IAuditedContext)}");
        });
    }
}