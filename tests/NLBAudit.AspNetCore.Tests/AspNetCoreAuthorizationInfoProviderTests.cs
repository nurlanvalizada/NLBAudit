using System.Security.Claims;
using NSubstitute;

namespace NLBAudit.AspNetCore.Tests;

public class AspNetCoreAuthorizationInfoProviderTests
{
    [Fact]
    public void IsAuthenticated_ReturnsTrue_WhenUserIsAuthenticated()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity([new Claim("sub", "1"), new Claim("username", "nurlan")], "TestAuth");
        context.User = new ClaimsPrincipal(identity);
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(context);

        var provider = new AspNetCoreAuthorizationInfoProvider(httpContextAccessor);

        // Act
        bool isAuthenticated = provider.IsAuthenticated();

        // Assert
        Assert.True(isAuthenticated);
    }
    
    [Fact]
    public void IsAuthenticated_ReturnsFalse_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(context);

        var provider = new AspNetCoreAuthorizationInfoProvider(httpContextAccessor);

        // Act
        bool isAuthenticated = provider.IsAuthenticated();

        // Assert
        Assert.False(isAuthenticated);
    }

    [Fact]
    public void GetUserName_ReturnsCorrectUserName_WhenSubClaimExists()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity([new Claim("sub", "42"), new Claim("username", "nurlan")], "TestAuth");
        context.User = new ClaimsPrincipal(identity);
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(context);

        var provider = new AspNetCoreAuthorizationInfoProvider(httpContextAccessor);

        // Act
        var userName = provider.GetUserName();

        // Assert
        Assert.Equal("nurlan", userName);
    }
    
    [Fact]
    public void GetUserId_ReturnsNull_WhenSubClaimNotExists()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity([], "TestAuth");
        context.User = new ClaimsPrincipal(identity);
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(context);

        var provider = new AspNetCoreAuthorizationInfoProvider(httpContextAccessor);

        // Act
        var userName = provider.GetUserName();

        // Assert
        Assert.Null(userName);
    }
}