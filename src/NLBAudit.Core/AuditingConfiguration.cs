namespace NLBAudit.Core;

public class AuditingConfiguration
{
    public bool IsEnabled { get; set; } = true;

    public bool IsEnabledForAnonymousUsers { get; set; } = false;

    public List<Type> IgnoredTypes { get; set; } = new();

    public bool SaveReturnValues { get; set; } = false;
}