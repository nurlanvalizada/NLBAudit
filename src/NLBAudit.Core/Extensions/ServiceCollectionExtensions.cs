using Microsoft.Extensions.DependencyInjection;

namespace NLBAudit.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureCoreAuditing<TUserId>(this IServiceCollection services, Action<AuditingConfiguration> action)
    {
        services.AddSingleton<AuditingConfiguration>(_ =>
        {
            var conf = new AuditingConfiguration();
            action(conf);
            return conf;
        });
        
        services.AddSingleton<IAuditingStore<TUserId>, LogAuditingStore<TUserId>>();
        services.AddSingleton<ICallerPartyInfoProvider, TestCallerPartyInfoProvider>();
        services.AddSingleton<IAuthorizationInfoProvider<TUserId>, TestAuthorizationInfoProvider<TUserId>>();
        services.AddScoped<IAuditingHelper<TUserId>, AuditingHelper<TUserId>>();
    }
}