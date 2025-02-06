using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using NLBAudit.Core.Attributes;

namespace NLBAudit.Core;

internal class AuditingHelper<TUserId>(
    ILogger<AuditingHelper<TUserId>> logger,
    IAuthorizationInfoProvider<TUserId> authorizationInfoProvider,
    IAuditingStore<TUserId> auditingStore,
    ICallerPartyInfoProvider callerPartyInfoProvider,
    AuditingConfiguration configuration)
    : IAuditingHelper<TUserId>
{
    public bool ShouldSaveAudit(MethodInfo? methodInfo, bool defaultValue = false)
    {
        if(!configuration.IsEnabled)
            return false;

        if(!configuration.IsEnabledForAnonymousUsers && !authorizationInfoProvider.IsAuthenticated())
        {
            return false;
        }

        if(methodInfo == null)
        {
            return false;
        }

        if(!methodInfo.IsPublic)
        {
            return false;
        }

        if(methodInfo.IsDefined(typeof(AuditedAttribute), true))
        {
            return true;
        }

        if(methodInfo.IsDefined(typeof(NotAuditedAttribute), true))
        {
            return false;
        }

        var classType = methodInfo.DeclaringType;
        if(classType != null)
        {
            if(classType.GetTypeInfo().IsDefined(typeof(AuditedAttribute), true))
            {
                return true;
            }

            if(classType.GetTypeInfo().IsDefined(typeof(NotAuditedAttribute), true))
            {
                return false;
            }
        }

        return defaultValue;
    }

    public AuditInfo<TUserId> CreateAuditInfo(Type type, MethodInfo method, object[] arguments)
    {
        return CreateAuditInfo(type, method, CreateArgumentsDictionary(method, arguments));
    }

    public AuditInfo<TUserId> CreateAuditInfo(Type? type, MethodInfo method, IDictionary<string, object?> arguments)
    {
        var auditInfo = new AuditInfo<TUserId>
        {
            UserId = authorizationInfoProvider.GetUserId(),
            ServiceName = type?.FullName ?? string.Empty,
            MethodName = method.Name,
            InputObj = ConvertArgumentsToJson(arguments),
            CreationTime = DateTime.Now,
            ClientIpAddress = callerPartyInfoProvider.ClientIpAddress,
            BrowserInfo = callerPartyInfoProvider.BrowserInfo,
        };

        return auditInfo;
    }

    public async Task SaveAsync(AuditInfo<TUserId> auditInfo, CancellationToken cancellationToken)
    {
        await auditingStore.SaveAsync(auditInfo, cancellationToken);
    }

    private string ConvertArgumentsToJson(IDictionary<string, object?>? arguments)
    {
        try
        {
            if(arguments is null || arguments.Count == 0)
            {
                return "{}";
            }

            var dictionary = new Dictionary<string, object?>();

            foreach(var argument in arguments)
            {
                if(argument.Value != null && configuration.IgnoredTypes.Any(t => t.IsInstanceOfType(argument.Value)))
                {
                    dictionary[argument.Key] = null;
                }
                else
                {
                    dictionary[argument.Key] = argument.Value;
                }
            }

            return JsonSerializer.Serialize(dictionary);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Could not serialize arguments to JSON.");
            return "{}";
        }
    }

    private static Dictionary<string, object?> CreateArgumentsDictionary(MethodInfo method, object[] arguments)
    {
        var parameters = method.GetParameters();
        var dictionary = new Dictionary<string, object?>();

        for(var i = 0; i < parameters.Length; i++)
        {
            var name = parameters[i].Name;
            if(name != null) 
                dictionary[name] = arguments[i];
        }

        return dictionary;
    }
}