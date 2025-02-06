using NLBAudit.Core.Attributes;

namespace NLBAudit.Core.UnitTests;

public class TestMethods
{
    public void NoAttributeMethod() { }
    
    [Audited]
    public void AuditedMethod() { }
    
    [NotAudited]
    public void NotAuditedMethod() { }
    
    
    private void PrivateMethod() { }

    public object MethodWithParametersAndReturnValue(string value1, int value2)
    {
        return new
        {
            value1 = value1,
        };
    }
    
    public void MethodWithParameters(string value1, DateTime value2)
    {
        
    }
}

[Audited]
public class ClassWithAudited
{
    public void MethodInAuditedClass() { }
}

[NotAudited]
public class ClassWithNotAudited
{
    public void MethodInNotAuditedClass() { }
}