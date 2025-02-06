using System.Reflection;

namespace NLBAudit.Core;

public interface IAuditingHelper<TUserId>
{
    bool ShouldSaveAudit(MethodInfo? methodInfo);

    AuditInfo<TUserId> CreateAuditInfo(Type type, MethodInfo method, object[] arguments);

    AuditInfo<TUserId> CreateAuditInfo(Type type, MethodInfo method, IDictionary<string, object?> arguments);

    Task SaveAsync(AuditInfo<TUserId> auditInfo, CancellationToken cancellationToken);
}