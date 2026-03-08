using NSubstitute;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.ApiKey.Entities;
using Domain.ApiKey.Ports.Out;
using FinalChallengeUsers.API.Middlewares;
using Infra.Password;

namespace UnitTests.Adapters.Middleware;

public class AuthenticationMiddlewareTest
{
    private const string JwtKey = "super-secret-key-for-testing-1234567890abc";
    private const string JwtIssuer = "test-issuer";
    private const string JwtAudience = "test-audience";

    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly ILogger<AuthenticationMiddleware> _logger;
    private readonly IConfiguration _configuration;
    private readonly AuthenticationMiddleware _sut;

    public AuthenticationMiddlewareTest()
    {
        _apiKeyRepository = Substitute.For<IApiKeyRepository>();
        _logger = Substitute.For<ILogger<AuthenticationMiddleware>>();
        _configuration = Substitute.For<IConfiguration>();

        var securitySettings = Options.Create(new SecuritySettings
        {
            JwtKey = JwtKey,
            JwtIssuer = JwtIssuer,
            JwtAudience = JwtAudience
        });

        _sut = new AuthenticationMiddleware(_apiKeyRepository, _logger, securitySettings, _configuration);
    }

    [Theory]
    [InlineData("/users/login")]
    [InlineData("/users")]
    [InlineData("/Users/Login")]
    [InlineData("/USERS")]
    public async Task InvokeAsync_WhenPathIsAllowed_ShouldCallNextWithoutCheckingAuth(string path)
    {
        // Arrange
        var context = CreateHttpContext(path);
        var nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };

        // Act
        await _sut.InvokeAsync(context, next);

        // Assert
        Assert.True(nextCalled);
        await _apiKeyRepository.DidNotReceive().GetByKeyAsync(Arg.Any<string>());
    }

    [Theory]
    [InlineData("/swagger/index.html")]
    [InlineData("/swagger/swagger.js")]
    public async Task InvokeAsync_WhenContainsSwaggerContent_ShouldBeAllowedWithoutAuth(string path)
    {
        // Arrange
        var context = CreateHttpContext(path);
        var nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };

        // Act
        await _sut.InvokeAsync(context, next);

        // Assert
        Assert.True(nextCalled);
        await _apiKeyRepository.DidNotReceive().GetByKeyAsync(Arg.Any<string>());
    }
    
    [Fact]
    public async Task InvokeAsync_WhenValidApiKey_ShouldCallNextAndSetApiKeyIdInContext()
    {
        // Arrange
        var apiKey = new ApiKey("valid-api-key", isActive: true);
        _apiKeyRepository.GetByKeyAsync("valid-api-key").Returns(apiKey);
        var context = CreateHttpContext("/plan/GetAll");
        context.Request.Headers["X-Api-Key"] = "valid-api-key";
        var nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };

        // Act
        await _sut.InvokeAsync(context, next);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal(apiKey.Id, context.Items["ApiKeyId"]);
    }

    [Fact]
    public async Task InvokeAsync_WhenApiKeyNotFound_ShouldFallThroughToJwtCheck()
    {
        // Arrange
        _apiKeyRepository.GetByKeyAsync("unknown-key").Returns((ApiKey)null!);
        var context = CreateHttpContext("/plan/GetAll");
        context.Request.Headers["X-Api-Key"] = "unknown-key";
        // No JWT either, so expect 401
        RequestDelegate next = _ => Task.CompletedTask;

        // Act
        await _sut.InvokeAsync(context, next);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WhenApiKeyRepositoryThrows_ShouldReturn500()
    {
        // Arrange
        _apiKeyRepository.GetByKeyAsync(Arg.Any<string>())
            .Returns<Task<ApiKey>>(_ => throw new Exception("DB error"));
        var context = CreateHttpContext("/plan/GetAll");
        context.Request.Headers["X-Api-Key"] = "error-key";
        RequestDelegate next = _ => Task.CompletedTask;

        // Act
        await _sut.InvokeAsync(context, next);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WhenValidJwtToken_ShouldCallNextAndSetUserId()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var token = CreateValidJwtToken(userId, "Test User");
        var context = CreateHttpContext("/plan/GetAll");
        context.Request.Headers["Authorization"] = $"Bearer {token}";
        var nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };

        // Act
        await _sut.InvokeAsync(context, next);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal(userId, context.Items["UserId"]);
    }

    [Fact]
    public async Task InvokeAsync_WhenValidJwtToken_ShouldSetClaimsPrincipal()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var token = CreateValidJwtToken(userId, "Test User");
        var context = CreateHttpContext("/plan/GetAll");
        context.Request.Headers["Authorization"] = $"Bearer {token}";
        RequestDelegate next = _ => Task.CompletedTask;

        // Act
        await _sut.InvokeAsync(context, next);

        // Assert
        Assert.NotNull(context.User);
        Assert.True(context.User.Identity?.IsAuthenticated);
    }

    [Fact]
    public async Task InvokeAsync_WhenJwtSignedWithWrongKey_ShouldReturn401()
    {
        // Arrange — token signed with a DIFFERENT key, causing SecurityTokenSignatureKeyNotFoundException
        var userId = Guid.NewGuid().ToString();
        var token = CreateJwtTokenWithKey(userId, "wrong-key-totally-different-from-config-abc123");
        var context = CreateHttpContext("/plan/GetAll");
        context.Request.Headers["Authorization"] = $"Bearer {token}";
        RequestDelegate next = _ => Task.CompletedTask;

        // Act
        await _sut.InvokeAsync(context, next);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WhenMalformedJwtToken_ShouldReturn500()
    {
        // Arrange — completely malformed token causes a generic Exception (not SecurityTokenException)
        var context = CreateHttpContext("/plan/GetAll");
        context.Request.Headers["Authorization"] = "Bearer totally.invalid.token";
        RequestDelegate next = _ => Task.CompletedTask;

        // Act
        await _sut.InvokeAsync(context, next);

        // Assert — middleware catches generic Exception and returns 500
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WhenNoAuthHeaders_ShouldReturn401()
    {
        // Arrange
        var context = CreateHttpContext("/plan/GetAll");
        RequestDelegate next = _ => Task.CompletedTask;

        // Act
        await _sut.InvokeAsync(context, next);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WhenAuthorizationHeaderIsEmpty_ShouldReturn401()
    {
        // Arrange
        var context = CreateHttpContext("/plan/GetAll");
        context.Request.Headers["Authorization"] = string.Empty;
        RequestDelegate next = _ => Task.CompletedTask;

        // Act
        await _sut.InvokeAsync(context, next);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WhenApiKeyIsWhitespace_ShouldFallThroughToJwtCheck()
    {
        // Arrange
        var context = CreateHttpContext("/plan/GetAll");
        context.Request.Headers["X-Api-Key"] = "   ";
        // No JWT either
        RequestDelegate next = _ => Task.CompletedTask;

        // Act
        await _sut.InvokeAsync(context, next);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        await _apiKeyRepository.DidNotReceive().GetByKeyAsync(Arg.Any<string>());
    }

    private static DefaultHttpContext CreateHttpContext(string path)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = new PathString(path);
        context.Response.Body = new System.IO.MemoryStream();
        return context;
    }

    private string CreateValidJwtToken(string userId, string userName)
        => CreateJwtTokenWithKey(userId, JwtKey, userName);

    private string CreateJwtTokenWithKey(string userId, string key, string userName = "Test User")
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key.PadRight(32, '!')));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userName)
        };
        var token = new JwtSecurityToken(
            issuer: JwtIssuer,
            audience: JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
