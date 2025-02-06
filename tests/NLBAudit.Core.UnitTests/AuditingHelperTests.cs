using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace NLBAudit.Core.UnitTests;

public class AuditingHelperTests
{
    private readonly ILogger<AuditingHelper<int>> _logger;
    private readonly IAuthorizationInfoProvider<int> _authorizationInfoProvider;
    private readonly IAuditingStore<int> _auditingStore;
    private readonly ICallerPartyInfoProvider _callerPartyInfoProvider;
    private readonly AuditingConfiguration _configuration;

    public AuditingHelperTests()
    {
        _logger = Substitute.For<ILogger<AuditingHelper<int>>>();
        _authorizationInfoProvider = Substitute.For<IAuthorizationInfoProvider<int>>();
        _auditingStore = Substitute.For<IAuditingStore<int>>();
        _callerPartyInfoProvider = Substitute.For<ICallerPartyInfoProvider>();

        // Default configuration for tests.
        _configuration = new AuditingConfiguration
        {
            IsEnabled = true,
            IsEnabledForAnonymousUsers = true,
            IgnoredTypes = [],
            SaveReturnValues = true
        };

        // Default caller info.
        _callerPartyInfoProvider.ClientIpAddress.Returns("127.0.0.1");
        _callerPartyInfoProvider.BrowserInfo.Returns("UnitTestBrowser");
    }

    private AuditingHelper<int> CreateHelper()
    {
        return new AuditingHelper<int>(_logger, _authorizationInfoProvider, _auditingStore, _callerPartyInfoProvider, _configuration);
    }

    #region ShouldSaveAudit Tests

    [Fact]
    public void ShouldSaveAudit_ReturnsFalse_WhenConfigurationDisabled()
    {
        // Arrange
        _configuration.IsEnabled = false;
        var helper = CreateHelper();
        var method = typeof(TestMethods).GetMethod(nameof(TestMethods.NoAttributeMethod));

        // Act
        var result = helper.ShouldSaveAudit(method);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldSaveAudit_ReturnsFalse_WhenAnonymousUser()
    {
        // Arrange
        _configuration.IsEnabledForAnonymousUsers = false;
        _authorizationInfoProvider.IsAuthenticated().Returns(false);
        var helper = CreateHelper();
        var method = typeof(TestMethods).GetMethod(nameof(TestMethods.NoAttributeMethod));

        // Act
        var result = helper.ShouldSaveAudit(method);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldSaveAudit_ReturnsFalse_WhenMethodInfoIsNull()
    {
        // Arrange
        var helper = CreateHelper();

        // Act
        var result = helper.ShouldSaveAudit(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldSaveAudit_ReturnsFalse_WhenMethodIsNotPublic()
    {
        // Arrange
        var helper = CreateHelper();
        var method = typeof(TestMethods).GetMethod("PrivateMethod", BindingFlags.Instance | BindingFlags.NonPublic);

        // Act
        var result = helper.ShouldSaveAudit(method);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldSaveAudit_ReturnsTrue_WhenMethodHasAuditedAttribute()
    {
        // Arrange
        var helper = CreateHelper();
        var method = typeof(TestMethods).GetMethod(nameof(TestMethods.AuditedMethod));

        // Act
        var result = helper.ShouldSaveAudit(method);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldSaveAudit_ReturnsFalse_WhenMethodHasNotAuditedAttribute()
    {
        // Arrange
        var helper = CreateHelper();
        var method = typeof(TestMethods).GetMethod(nameof(TestMethods.NotAuditedMethod));

        // Act
        var result = helper.ShouldSaveAudit(method);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldSaveAudit_ReturnsTrue_WhenDeclaringClassHasAuditedAttribute()
    {
        // Arrange
        var helper = CreateHelper();
        var method = typeof(ClassWithAudited).GetMethod(nameof(ClassWithAudited.MethodInAuditedClass));

        // Act
        var result = helper.ShouldSaveAudit(method);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldSaveAudit_ReturnsFalse_WhenDeclaringClassHasNotAuditedAttribute()
    {
        // Arrange
        var helper = CreateHelper();
        var method = typeof(ClassWithNotAudited).GetMethod(nameof(ClassWithNotAudited.MethodInNotAuditedClass));

        // Act
        var result = helper.ShouldSaveAudit(method);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldSaveAudit_ReturnsTrue_WhenNoAttributeSpecified()
    {
        // Arrange
        var helper = CreateHelper();
        var method = typeof(TestMethods).GetMethod(nameof(TestMethods.NoAttributeMethod));

        // Act
        var result = helper.ShouldSaveAudit(method);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region CreateAuditInfo Tests

    [Fact]
    public void CreateAuditInfo_SetsPropertiesCorrectly_WithArgumentsArray()
    {
        // Arrange
        int expectedUserId = 42;
        _authorizationInfoProvider.GetUserId().Returns(expectedUserId);

        var helper = CreateHelper();
        var method = typeof(TestMethods).GetMethod(nameof(TestMethods.MethodWithParametersAndReturnValue));
        object[] args = ["value1", 123];

        // Act
        var auditInfo = helper.CreateAuditInfo(typeof(TestMethods), method, args);

        // Assert
        Assert.Equal(expectedUserId, auditInfo.UserId);
        Assert.Equal(typeof(TestMethods).FullName, (string?)auditInfo.ServiceName);
        Assert.Equal(method.Name, (string?)auditInfo.MethodName);
        var deserialized = JsonSerializer.Deserialize<Dictionary<string, object>>(auditInfo.InputObj);
        Assert.NotNull(deserialized);
        Assert.Equal(2, deserialized.Count);
    }

    [Fact]
    public void CreateAuditInfo_SetsPropertiesCorrectly_WithArgumentsDictionary_AndHandlesIgnoredTypes()
    {
        // Arrange
        int expectedUserId = 100;
        _authorizationInfoProvider.GetUserId().Returns(expectedUserId);

        // Configure an ignored type (for example, DateTime)
        _configuration.IgnoredTypes.Add(typeof(DateTime));

        var helper = CreateHelper();
        var method = typeof(TestMethods).GetMethod(nameof(TestMethods.MethodWithParameters));
        var arguments = new Dictionary<string, object?>
        {
            { "value1", "test" },
            { "value2", DateTime.Now }
        };

        // Act
        var auditInfo = helper.CreateAuditInfo(typeof(TestMethods), method, arguments);

        // Assert
        Assert.Equal(expectedUserId, auditInfo.UserId);
        Assert.Equal(typeof(TestMethods).FullName, (string?)auditInfo.ServiceName);
        Assert.Equal(method.Name, (string?)auditInfo.MethodName);

        // Deserialize and check that the DateTime parameter was replaced with null.
        var deserialized = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(auditInfo.InputObj);
        Assert.NotNull(deserialized);
        Assert.Equal("test", deserialized["value1"].GetString());
        Assert.True(deserialized["value2"].ValueKind == JsonValueKind.Null);
    }

    #endregion

    #region SaveAsync Tests

    [Fact]
    public async Task SaveAsync_CallsAuditingStoreSaveAsync()
    {
        // Arrange
        var helper = CreateHelper();
        var auditInfo = new AuditInfo<int>
        {
            UserId = 1,
            ServiceName = "TestService",
            MethodName = "TestMethod",
            InputObj = "{}",
            CreationTime = DateTime.Now,
            ClientIpAddress = "127.0.0.1",
            BrowserInfo = "UnitTestBrowser",
        };

        // Act
        await helper.SaveAsync(auditInfo, CancellationToken.None);

        // Assert
        await _auditingStore.Received(1).SaveAsync(auditInfo, Arg.Any<CancellationToken>());
    }

    #endregion
}