namespace NLBAudit.Core.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class NotAuditedAttribute : Attribute;