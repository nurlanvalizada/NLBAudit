using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using NLBAudit.Core.UnitTests.Setup;
using NSubstitute;

namespace NLBAudit.Core.UnitTests;

public class AuditingHelperTests
{
    private readonly ILogger<AuditingHelper> _logger;
    private readonly IAuthorizationInfoProvider _authorizationInfoProvider;
    private readonly IAuditingStore _auditingStore;
    private readonly ICallerPartyInfoProvider _callerPartyInfoProvider;
    private readonly AuditingConfiguration _configuration;

    public AuditingHelperTests()
    {
        _logger = Substitute.For<ILogger<AuditingHelper>>();
        _authorizationInfoProvider = Substitute.For<IAuthorizationInfoProvider>();
        _auditingStore = Substitute.For<IAuditingStore>();
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

    private AuditingHelper CreateHelper()
    {
        return new AuditingHelper(_logger, _authorizationInfoProvider, _auditingStore, _callerPartyInfoProvider, _configuration);
    }

    #region ShouldSaveAudit Tests

    [Fact]
    public void ShouldSaveAudit_ReturnsFalse_WhenConfigurationDisabled()
    {
        // Arrange
        _configuration.IsEnabled = false;
        var helper = CreateHelper();
        var method = typeof(ClassWithVariousMethods).GetMethod(nameof(ClassWithVariousMethods.NoAttributeMethod));

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
        var method = typeof(ClassWithVariousMethods).GetMethod(nameof(ClassWithVariousMethods.NoAttributeMethod));

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
        var method = typeof(ClassWithVariousMethods).GetMethod("PrivateMethod", BindingFlags.Instance | BindingFlags.NonPublic);

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
        var method = typeof(ClassWithVariousMethods).GetMethod(nameof(ClassWithVariousMethods.AuditedMethod));

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
        var method = typeof(ClassWithVariousMethods).GetMethod(nameof(ClassWithVariousMethods.NotAuditedMethod));

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
        var method = typeof(ClassWithVariousMethods).GetMethod(nameof(ClassWithVariousMethods.NoAttributeMethod));

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
        string expectedUserName = "test";
        _authorizationInfoProvider.GetUserName().Returns(expectedUserName);

        var helper = CreateHelper();
        var method = typeof(ClassWithVariousMethods).GetMethod(nameof(ClassWithVariousMethods.MethodWithParametersAndReturnValue));
        object[] args = ["value1", 123];

        // Act
        var auditInfo = helper.CreateAuditInfo("/test", "GET", typeof(ClassWithVariousMethods), method, args);

        // Assert
        Assert.Equal(expectedUserName, auditInfo.UserName);
        Assert.Equal(typeof(ClassWithVariousMethods).FullName, (string?)auditInfo.ServiceName);
        Assert.Equal(method.Name, (string?)auditInfo.MethodName);
        var deserialized = JsonSerializer.Deserialize<Dictionary<string, object>>(auditInfo.InputObj);
        Assert.NotNull(deserialized);
        Assert.Equal(2, deserialized.Count);
    }

    [Fact]
    public void CreateAuditInfo_SetsPropertiesCorrectly_WithArgumentsDictionary_AndHandlesIgnoredTypes()
    {
        // Arrange
        string expectedUserName = "test";
        _authorizationInfoProvider.GetUserName().Returns(expectedUserName);

        // Configure an ignored type (for example, DateTime)
        _configuration.IgnoredTypes.Add(typeof(DateTime));

        var helper = CreateHelper();
        var method = typeof(ClassWithVariousMethods).GetMethod(nameof(ClassWithVariousMethods.MethodWithParameters));
        var arguments = new Dictionary<string, object?>
        {
            { "value1", "test" },
            { "value2", DateTime.Now }
        };

        // Act
        var auditInfo = helper.CreateAuditInfo("/test", "GET", typeof(ClassWithVariousMethods), method, arguments);

        // Assert
        Assert.Equal(expectedUserName, auditInfo.UserName);
        Assert.Equal(typeof(ClassWithVariousMethods).FullName, (string?)auditInfo.ServiceName);
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
        var auditInfo = new AuditInfo
        {
            UserName = "test",
            Path = "/test",
            HttpMethod = "GET",
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