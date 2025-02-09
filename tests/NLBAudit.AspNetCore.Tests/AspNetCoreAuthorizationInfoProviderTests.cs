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
        var identity = new ClaimsIdentity([new Claim("sub", "1")], "TestAuth");
        context.User = new ClaimsPrincipal(identity);
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(context);

        var provider = new AspNetCoreAuthorizationInfoProvider<int>(httpContextAccessor);

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

        var provider = new AspNetCoreAuthorizationInfoProvider<int>(httpContextAccessor);

        // Act
        bool isAuthenticated = provider.IsAuthenticated();

        // Assert
        Assert.False(isAuthenticated);
    }

    [Fact]
    public void GetUserId_ReturnsCorrectUserId_WhenSubClaimExists()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity([new Claim("sub", "42")], "TestAuth");
        context.User = new ClaimsPrincipal(identity);
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(context);

        var provider = new AspNetCoreAuthorizationInfoProvider<int>(httpContextAccessor);

        // Act
        int? userId = provider.GetUserId();

        // Assert
        Assert.Equal(42, userId);
    }
    
    [Fact]
    public void GetUserId_ReturnsZero_WhenSubClaimNotExists()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity([], "TestAuth");
        context.User = new ClaimsPrincipal(identity);
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(context);

        var provider = new AspNetCoreAuthorizationInfoProvider<int>(httpContextAccessor);

        // Act
        int? userId = provider.GetUserId();

        // Assert
        Assert.Equal(0, userId);
    }
}