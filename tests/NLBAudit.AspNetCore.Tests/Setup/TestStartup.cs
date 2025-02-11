using Microsoft.Extensions.DependencyInjection.Extensions;
using NLBAudit.AspNetCore.Mvc.Extensions;
using NLBAudit.Core;
using NLBAudit.Core.Extensions;

namespace NLBAudit.AspNetCore.Tests.Setup;

public class TestStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        
        services.ConfigureCoreAuditing<int>(config =>
        {
            config.IsEnabled = true;
            config.IsEnabledForAnonymousUsers = true;
            config.SaveReturnValues = true;
        });

        services.Replace(ServiceDescriptor.Singleton<ICallerPartyInfoProvider, AspNetCoreCallerPartyInfoProvider>());
        
        services.AddControllers(options =>
        {
            options.AddAuditingFilter<int>();
        });
    }

    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.ConfigureTestMinimalApi();
        });
    }
}