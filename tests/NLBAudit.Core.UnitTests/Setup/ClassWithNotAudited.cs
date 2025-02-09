using NLBAudit.Core.Attributes;

namespace NLBAudit.Core.UnitTests.Setup;

[NotAudited]
public class ClassWithNotAudited
{
    public void MethodInNotAuditedClass() { }
}