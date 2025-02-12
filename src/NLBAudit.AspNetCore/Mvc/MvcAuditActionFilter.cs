using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using NLBAudit.AspNetCore.Extensions;
using NLBAudit.Core;

namespace NLBAudit.AspNetCore.Mvc;

public class MvcAuditActionFilter(IAuditingHelper auditingHelper,
                                  IServiceScopeFactory serviceScopeFactory,
                                  AuditingConfiguration configuration) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!ShouldSaveAudit(context))
        {
            await next();
            return;
        }
        
        if(context.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor)
        {
            await next();
            return;
        }

        var auditInfo = auditingHelper.CreateAuditInfo(
            context.HttpContext.GetFullUrl(),
            context.HttpContext.Request.Method,
            controllerActionDescriptor.ControllerTypeInfo.AsType(),
            controllerActionDescriptor.MethodInfo,
            context.ActionArguments
        );

        var stopwatch = Stopwatch.StartNew();

        ActionExecutedContext? result = null;
        try
        {
            result = await next();
            if (result is { Exception: not null, ExceptionHandled: false })
            {
                auditInfo.Exception = result.Exception;
            }
        }
        catch (Exception ex)
        {
            auditInfo.Exception = ex;
        }
        finally
        {
            stopwatch.Stop();
            auditInfo.Duration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

            if (configuration.SaveReturnValues && result != null)
            {
                switch (result.Result)
                {
                    case ObjectResult objectResult:
                        auditInfo.ReturnValue = JsonSerializer.Serialize(objectResult.Value);
                        break;

                    case JsonResult jsonResult:
                        auditInfo.ReturnValue = JsonSerializer.Serialize(jsonResult.Value ?? new { });
                        break;

                    case ContentResult contentResult:
                        auditInfo.ReturnValue = contentResult.Content;
                        break;
                }
            }
            
            _ = Task.Run(async () =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var auditingStore = scope.ServiceProvider.GetRequiredService<IAuditingStore>();

                var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                await auditingStore.SaveAsync(auditInfo, cancellationTokenSource.Token);
            });
        }
    }

    private bool ShouldSaveAudit(ActionExecutingContext actionContext)
    {
        var controllerActionDescriptor = actionContext.ActionDescriptor as ControllerActionDescriptor;
        return controllerActionDescriptor != null && auditingHelper.ShouldSaveAudit(controllerActionDescriptor.MethodInfo);
    }
}