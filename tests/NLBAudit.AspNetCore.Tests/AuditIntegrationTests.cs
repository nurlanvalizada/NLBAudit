using NLBAudit.AspNetCore.Tests.Setup;
using NLBAudit.Core;

namespace NLBAudit.AspNetCore.Tests;

public class AuditIntegrationTests(TestApplicationFactory factory) : IClassFixture<TestApplicationFactory>
{
    [Fact]
    public async Task AuditActionFilter_RecordsAudit_WhenControllerActionIsCalled()
    {
        HttpClient client = factory.CreateClient();
        
        int testId = 42;
        HttpResponseMessage response = await client.GetAsync($"/api/test/{testId}");
        response.EnsureSuccessStatusCode();
        
        var auditingStore = factory.Services.GetRequiredService<IAuditingStore>() as LogAuditingStore;
        Assert.NotNull(auditingStore);
        Assert.NotNull(auditingStore.LastAuditInfo);
        
        AuditInfo auditInfo = auditingStore.LastAuditInfo!;
        Assert.Equal("test", auditInfo.UserName);
        
        Assert.Contains("TestController", auditInfo.ServiceName);
        Assert.Equal("GetTest", auditInfo.MethodName);
        
        Assert.Contains($"{testId}", auditInfo.InputObj);
    }
    
    [Fact]
    public async Task AuditActionFilter_RecordsAudit_WhenMinimalApiEndpointIsCalled()
    {
        HttpClient client = factory.CreateClient();
        
        string testData = "Test";
        HttpResponseMessage response = await client.GetAsync("/weatherforecast?testInfo=" + testData);
        response.EnsureSuccessStatusCode();
        
        var auditingStore = factory.Services.GetRequiredService<IAuditingStore>() as LogAuditingStore;
        Assert.NotNull(auditingStore);
        Assert.NotNull(auditingStore.LastAuditInfo);
        
        AuditInfo auditInfo = auditingStore.LastAuditInfo!;
        Assert.Equal("test", auditInfo.UserName);
        
        Assert.Contains("TestMinimalApis", auditInfo.ServiceName);
        Assert.Contains("ConfigureTestMinimalApi", auditInfo.MethodName);
        
        Assert.Contains($"{testData}", auditInfo.InputObj);
    }
}