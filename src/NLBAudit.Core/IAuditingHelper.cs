using System.Reflection;

namespace NLBAudit.Core;

public interface IAuditingHelper
{
    bool ShouldSaveAudit(MethodInfo? methodInfo, bool allowInternalMethods = false);

    AuditInfo CreateAuditInfo(string path, string httpMethod, Type type, MethodInfo method, object[] arguments);

    AuditInfo CreateAuditInfo(string path, string httpMethod, Type type, MethodInfo method, IDictionary<string, object?> arguments);

    Task SaveAsync(AuditInfo auditInfo, CancellationToken cancellationToken);
}