using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NLBAudit.Core;

namespace NLBAudit.Store.EfCore.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureAuditingEfCoreStore<TUserId, TDbContext>(this IServiceCollection services) where TDbContext : DbContext
    {
        services.Replace(ServiceDescriptor.Scoped<IAuditingStore<TUserId>, EfCoreAuditingStore<TUserId>>());
        services.AddScoped<IAuditedContext<TUserId>>(provider =>
        {
            var service = provider.GetService<TDbContext>();
            if(service is IAuditedContext<TUserId> adc)
            {
                return adc;
            }

            throw new InvalidOperationException($"The DbContext {typeof(TDbContext).Name} must implement {typeof(IAuditedContext<TUserId>).Name}");
        });
    }
}