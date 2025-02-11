using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using NLBAudit.Core.Attributes;

namespace NLBAudit.Core;

public class AuditingHelper<TUserId>(
    ILogger<AuditingHelper<TUserId>> logger,
    IAuthorizationInfoProvider<TUserId> authorizationInfoProvider,
    IAuditingStore<TUserId> auditingStore,
    ICallerPartyInfoProvider callerPartyInfoProvider,
    AuditingConfiguration configuration)
    : IAuditingHelper<TUserId>
{
    public bool ShouldSaveAudit(MethodInfo? methodInfo, bool allowInternalMethods = false)
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

        if(!methodInfo.IsPublic && !allowInternalMethods)
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

        return true;
    }

    public AuditInfo<TUserId> CreateAuditInfo(string path, string httpMethod, Type type, MethodInfo method, object[] arguments)
    {
        return CreateAuditInfo(path, httpMethod, type, method, CreateArgumentsDictionary(method, arguments));
    }

    public AuditInfo<TUserId> CreateAuditInfo(string path, string httpMethod, Type type, MethodInfo method, IDictionary<string, object?> arguments)
    {
        var correctedArguments = CorrectArguments(method, arguments);
        var auditInfo = new AuditInfo<TUserId>
        {
            UserId = authorizationInfoProvider.GetUserId(),
            Path = path,
            HttpMethod = httpMethod,
            ServiceName = type.FullName ?? type.Name,
            MethodName = method.Name,
            InputObj = ConvertArgumentsToJson(correctedArguments),
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

    private static Dictionary<string, object?> CreateArgumentsDictionary(MethodInfo method, object?[] arguments)
    {
        var methodParameters = method.GetParameters();
        var dictionary = new Dictionary<string, object?>();
        
        if(methodParameters.Length != arguments.Length)
        {
            throw new ArgumentException("The number of parameters does not match the number of arguments for the method:" + method.Name);
        }

        for(var i = 0; i < methodParameters.Length; i++)
        {
            var name = methodParameters[i].Name;
            if(name != null) 
                dictionary[name] = arguments[i];
        }

        return dictionary;
    }

    private static Dictionary<string, object?> CorrectArguments(MethodInfo method, IDictionary<string, object?> parameters)
    {
        var methodArguments = parameters.Select(p => p.Value).ToArray();
        var dictionary = CreateArgumentsDictionary(method, methodArguments);
        
        if(!dictionary.Keys.SequenceEqual(parameters.Keys))
        {
            throw new ArgumentException("The parameter names do not match the method parameters.");
        }

        return dictionary;
    }
}