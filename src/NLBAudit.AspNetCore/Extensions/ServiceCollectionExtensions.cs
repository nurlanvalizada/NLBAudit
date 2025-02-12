using Microsoft.Extensions.DependencyInjection.Extensions;
using NLBAudit.Core;
using NLBAudit.Core.Extensions;

namespace NLBAudit.AspNetCore.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureAspNetCoreAuditing<TUserId>(this IServiceCollection services, Action<AuditingConfiguration> action)
    {
        services.ConfigureCoreAuditing<TUserId>(action);

        services.Replace(ServiceDescriptor.Singleton<ICallerPartyInfoProvider, AspNetCoreCallerPartyInfoProvider>());
        services.Replace(ServiceDescriptor.Singleton<IAuthorizationInfoProvider, AspNetCoreAuthorizationInfoProvider>());
    }
}