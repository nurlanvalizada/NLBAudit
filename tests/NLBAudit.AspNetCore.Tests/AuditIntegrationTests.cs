using Microsoft.AspNetCore.Mvc.Testing;
using NLBAudit.AspNetCore.Tests.Setup;
using NLBAudit.Core;

namespace NLBAudit.AspNetCore.Tests;

public class AuditIntegrationTests(TestApplicationFactory factory) : IClassFixture<TestApplicationFactory>
{
    [Fact]
    public async Task AuditActionFilter_RecordsAudit_WhenEndpointIsCalled()
    {
        HttpClient client = factory.CreateClient();

        // Act: Call the dummy endpoint (this triggers the audit filter).
        int testId = 42;
        HttpResponseMessage response = await client.GetAsync($"/api/test/{testId}");
        response.EnsureSuccessStatusCode();
        
        var auditingStore = factory.Services.GetRequiredService<IAuditingStore<int>>() as LogAuditingStore<int>;
        Assert.NotNull(auditingStore);
        Assert.NotNull(auditingStore.LastAuditInfo);

        // Optionally, verify that the audit info contains expected values.
        AuditInfo<int> auditInfo = auditingStore.LastAuditInfo!;
        Assert.Equal(1, auditInfo.UserId);
        Assert.Contains("TestController", auditInfo.ServiceName);
        // The action method name should match the method in your controller.
        Assert.Equal("GetTest", auditInfo.MethodName);
        Assert.Contains($"{testId}", auditInfo.InputObj);
    }
}