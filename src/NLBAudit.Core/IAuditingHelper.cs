using System.Reflection;

namespace NLBAudit.Core;

public interface IAuditingHelper<TUserId>
{
    bool ShouldSaveAudit(MethodInfo? methodInfo, bool allowInternalMethods = false);

    AuditInfo<TUserId> CreateAuditInfo(string path, string httpMethod, Type type, MethodInfo method, object[] arguments);

    AuditInfo<TUserId> CreateAuditInfo(string path, string httpMethod, Type type, MethodInfo method, IDictionary<string, object?> arguments);

    Task SaveAsync(AuditInfo<TUserId> auditInfo, CancellationToken cancellationToken);
}