using NLBAudit.Core.Attributes;

namespace NLBAudit.Core.UnitTests.Setup;

[Audited]
public class ClassWithAudited
{
    public void MethodInAuditedClass() { }
}