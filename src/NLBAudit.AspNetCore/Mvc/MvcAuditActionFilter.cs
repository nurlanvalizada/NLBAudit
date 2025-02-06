using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using NLBAudit.Core;

namespace NLBAudit.AspNetCore.Mvc;

public class MvcAuditActionFilter<TUserId>(
    AuditingConfiguration configuration,
    IAuditingHelper<TUserId> auditingHelper)
    : IAsyncActionFilter
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
            controllerActionDescriptor.ControllerTypeInfo.AsType(),
            controllerActionDescriptor.MethodInfo,
            context.ActionArguments
        );

        var stopwatch = Stopwatch.StartNew();

        ActionExecutedContext result = null;
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
            auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

            if (configuration.SaveReturnValues && result != null)
            {
                switch (result.Result)
                {
                    case ObjectResult objectResult:
                        auditInfo.ReturnValue = JsonSerializer.Serialize(objectResult.Value);
                        break;

                    case JsonResult jsonResult:
                        auditInfo.ReturnValue = JsonSerializer.Serialize(jsonResult.Value);
                        break;

                    case ContentResult contentResult:
                        auditInfo.ReturnValue = contentResult.Content;
                        break;
                }
            }

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await auditingHelper.SaveAsync(auditInfo, cancellationTokenSource.Token);
        }
    }

    private bool ShouldSaveAudit(ActionExecutingContext actionContext)
    {
        if(!configuration.IsEnabled)
            return false;
            
        if(actionContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            return auditingHelper.ShouldSaveAudit(controllerActionDescriptor.MethodInfo, true);
        }
        
        return false;
    }
}