using Microsoft.EntityFrameworkCore;
using NLBAudit.Core;
using NLBAudit.Store.EfCore.IntegrationTests.Setup;

namespace NLBAudit.Store.EfCore.IntegrationTests;

public class EfCoreAuditingStoreIntegrationTests
{
    [Fact]
    public async Task SaveAsync_ShouldAddAuditLogEntity_ToDatabase()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestAuditedContext>().UseInMemoryDatabase(databaseName: "TestDb").Options;
        
        await using (var context = new TestAuditedContext(options))
        {
            var auditingStore = new EfCoreAuditingStore<int>(context);
            var auditInfo = new AuditInfo<int>
            {
                UserId = 1,
                ServiceName = "TestService",
                MethodName = "TestMethod",
                InputObj = "{\"param\":\"value\"}",
                CreationTime = DateTime.UtcNow,
                Duration = 10,
                Exception = null,
                BrowserInfo = "TestBrowser",
                ClientIpAddress = "127.0.0.1"
            };

            // Act: Save the audit info.
            await auditingStore.SaveAsync(auditInfo, CancellationToken.None);
        }

        // Assert: Create a new context instance to verify that the entity was saved.
        await using (var verificationContext = new TestAuditedContext(options))
        {
            var count = await verificationContext.AuditLogs.CountAsync();
            Assert.Equal(1, count);

            var savedEntity = await verificationContext.AuditLogs.FirstOrDefaultAsync();
            Assert.NotNull(savedEntity);
            Assert.Equal(1, savedEntity.UserId);
            Assert.Equal("TestService", savedEntity.ServiceName);
            Assert.Equal("TestMethod", savedEntity.MethodName);
            Assert.Equal("{\"param\":\"value\"}", savedEntity.InputObj);
        }
    }
}