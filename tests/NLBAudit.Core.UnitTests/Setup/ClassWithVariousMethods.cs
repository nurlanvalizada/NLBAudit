using NLBAudit.Core.Attributes;

namespace NLBAudit.Core.UnitTests.Setup;

public class ClassWithVariousMethods
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