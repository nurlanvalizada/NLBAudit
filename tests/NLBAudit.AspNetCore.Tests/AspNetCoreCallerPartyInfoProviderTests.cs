using System.Net;
using NSubstitute;

namespace NLBAudit.AspNetCore.Tests;

public class AspNetCoreCallerPartyInfoProviderTests
{
    [Fact]
    public void ClientIpAddress_ReturnsXForwardedForIp_WhenHeaderExists()
    {
        // Arrange: Create a HttpContext with the X-Forwarded-For header.
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Forwarded-For"] = "203.0.113.5, proxy1, proxy2";
        // Set a fallback IP address.
        context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
        var logger = Substitute.For<ILogger<AspNetCoreCallerPartyInfoProvider>>();
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(context);

        var provider = new AspNetCoreCallerPartyInfoProvider(httpContextAccessor, logger);

        // Act
        string? clientIp = provider.ClientIpAddress;

        // Assert: Expect the first IP from X-Forwarded-For.
        Assert.Equal("203.0.113.5", clientIp);
    }

    [Fact]
    public void BrowserInfo_ReturnsUserAgent_WhenHeaderExists()
    {
        // Arrange: Create a HttpContext with a User-Agent header.
        var context = new DefaultHttpContext();
        context.Request.Headers["User-Agent"] = "TestBrowser";
        var logger = Substitute.For<ILogger<AspNetCoreCallerPartyInfoProvider>>();
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(context);

        var provider = new AspNetCoreCallerPartyInfoProvider(httpContextAccessor, logger);

        // Act
        string? browserInfo = provider.BrowserInfo;

        // Assert
        Assert.Equal("TestBrowser", browserInfo);
    }
}