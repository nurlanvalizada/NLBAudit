using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using NLBAudit.Core;

namespace NLBAudit.AspNetCore.MinimalApis;

public class MinimalApiEndpointAuditFilter<TUserId>(IAuditingHelper<TUserId> auditingHelper, AuditingConfiguration configuration) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!ShouldSaveAudit(context))
        {
            return await next(context);
        }
        
        var endpoint = context.HttpContext.GetEndpoint();
        if(endpoint is null)
        {
            return await next(context);
        }

        var routeName = endpoint.DisplayName ?? "MinimalAPI Route";
        var methodInfo = endpoint.Metadata.GetMetadata<MethodInfo>();
        var controllerType = methodInfo?.DeclaringType;
        
        var auditInfo = auditingHelper.CreateAuditInfo(
            controllerType,
            methodInfo,
            context.Arguments.ToArray()
        );

        var stopwatch = Stopwatch.StartNew();
        object? result = null;

        try
        {
            result = await next(context);
        }
        catch (Exception ex)
        {
            auditInfo.Exception = ex;
        }
        finally
        {
            stopwatch.Stop();
            auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
            
            if (result != null)
            {
               auditInfo.ReturnValue = JsonSerializer.Serialize(result);
            }

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await auditingHelper.SaveAsync(auditInfo, cancellationTokenSource.Token);
        }

        return result;
    }

    private bool ShouldSaveAudit(EndpointFilterInvocationContext context)
    {
        if(!configuration.IsEnabled)
            return false;
        
        var endpoint = context.HttpContext.GetEndpoint();
        if(endpoint is not null)
        {
            var methodInfo = endpoint.Metadata.OfType<MethodInfo>().FirstOrDefault();
            if(methodInfo is not null)
            {
                return auditingHelper.ShouldSaveAudit(methodInfo, true);
            }
        }
        
        return false;
    }
}