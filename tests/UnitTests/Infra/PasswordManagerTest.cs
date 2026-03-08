using NSubstitute;
using Microsoft.Extensions.Configuration;
using Infra.Password;
using Domain.Users.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace UnitTests.Infra;

public class PasswordManagerTest
{
    private const string SecretKey = "super-secret-key-for-testing-1234567890abc";
    private const string JwtKey = "jwt-signing-key-for-testing-1234567890abcdefgh";
    private const string JwtIssuer = "test-issuer";
    private const string JwtAudience = "test-audience";

    private readonly IConfiguration _configuration;
    private readonly PasswordManager _sut;

    public PasswordManagerTest()
    {
        _configuration = Substitute.For<IConfiguration>();
        _configuration["Security:Key"].Returns(SecretKey);
        _configuration["Jwt:Key"].Returns(JwtKey);
        _configuration["Jwt:Issuer"].Returns(JwtIssuer);
        _configuration["Jwt:Audience"].Returns(JwtAudience);
        _configuration["Jwt:ExpirationMinutes"].Returns("60");

        _sut = new PasswordManager(_configuration);
    }

    // --- CreatePasswordHash ---

    [Fact]
    public void CreatePasswordHash_ShouldReturnNonEmptyHash()
    {
        _sut.CreatePasswordHash("minha-senha", out var hash);

        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }

    [Fact]
    public void CreatePasswordHash_SamePasword_ShouldProduceSameHash()
    {
        _sut.CreatePasswordHash("minha-senha", out var hash1);
        _sut.CreatePasswordHash("minha-senha", out var hash2);

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void CreatePasswordHash_DifferentPasswords_ShouldProduceDifferentHashes()
    {
        _sut.CreatePasswordHash("senha-A", out var hash1);
        _sut.CreatePasswordHash("senha-B", out var hash2);

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void CreatePasswordHash_ShouldProduceBase64EncodedHash()
    {
        _sut.CreatePasswordHash("teste123", out var hash);

        // Should not throw
        var bytes = Convert.FromBase64String(hash);
        Assert.NotEmpty(bytes);
    }

    // --- VerifyPassword ---

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        const string password = "senha-correta";
        _sut.CreatePasswordHash(password, out var hash);

        var result = _sut.VerifyPassword(password, hash);

        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_WithWrongPassword_ShouldReturnFalse()
    {
        _sut.CreatePasswordHash("senha-original", out var hash);

        var result = _sut.VerifyPassword("senha-errada", hash);

        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_WithEmptyPassword_ShouldNotMatchNonEmptyHash()
    {
        _sut.CreatePasswordHash("senha", out var hash);

        var result = _sut.VerifyPassword("", hash);

        Assert.False(result);
    }

    // --- GenerateJwtToken ---

    [Fact]
    public void GenerateJwtToken_ShouldReturnNonEmptyToken()
    {
        var user = new User("João", "Silva", "joao@email.com", new DateTime(1990, 1, 1));

        var token = _sut.GenerateJwtToken(user);

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void GenerateJwtToken_ShouldReturnValidJwtFormat()
    {
        var user = new User("Maria", "Costa", "maria@email.com", new DateTime(1985, 6, 15));

        var token = _sut.GenerateJwtToken(user);

        // JWT has 3 parts separated by dots
        var parts = token.Split('.');
        Assert.Equal(3, parts.Length);
    }

    [Fact]
    public void GenerateJwtToken_ShouldContainUserIdClaim()
    {
        var user = new User("Carlos", "Lima", "carlos@email.com", new DateTime(1995, 3, 20));

        var token = _sut.GenerateJwtToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var nameIdentifierClaim = jwtToken.Claims
            .FirstOrDefault(c => c.Type == "nameid" || c.Type.Contains("nameidentifier"));

        Assert.NotNull(nameIdentifierClaim);
        Assert.Equal(user.Id.ToString(), nameIdentifierClaim!.Value);
    }

    [Fact]
    public void GenerateJwtToken_ShouldContainUserNameClaim()
    {
        var user = new User("Ana", "Pereira", "ana@email.com", new DateTime(2000, 12, 31));

        var token = _sut.GenerateJwtToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var nameClaim = jwtToken.Claims
            .FirstOrDefault(c => c.Type == "unique_name" || c.Type == "name");

        Assert.NotNull(nameClaim);
        Assert.Equal(user.Name, nameClaim!.Value);
    }

    [Fact]
    public void GenerateJwtToken_ShouldHaveFutureExpiration()
    {
        var user = new User("Pedro", "Alves", "pedro@email.com", new DateTime(1988, 7, 4));

        var token = _sut.GenerateJwtToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
    }

    [Fact]
    public void GenerateJwtToken_DifferentUsers_ShouldProduceDifferentTokens()
    {
        var user1 = new User("User1", "Last1", "user1@email.com", new DateTime(1990, 1, 1));
        var user2 = new User("User2", "Last2", "user2@email.com", new DateTime(1990, 1, 1));

        var token1 = _sut.GenerateJwtToken(user1);
        var token2 = _sut.GenerateJwtToken(user2);

        Assert.NotEqual(token1, token2);
    }

    // --- Validação de chaves ausentes ou inválidas ---

    [Fact]
    public void GenerateJwtToken_WhenJwtKeyIsEmpty_ShouldThrowInvalidOperationException()
    {
        var config = Substitute.For<IConfiguration>();
        config["Jwt:Key"].Returns(string.Empty);
        var sut = new PasswordManager(config);
        var user = new User("Test", "User", "test@email.com", new DateTime(1990, 1, 1));

        Assert.Throws<InvalidOperationException>(() => sut.GenerateJwtToken(user));
    }

    [Fact]
    public void GenerateJwtToken_WhenJwtKeyIsTooShort_ShouldThrowInvalidOperationException()
    {
        var config = Substitute.For<IConfiguration>();
        config["Jwt:Key"].Returns("short-key");
        var sut = new PasswordManager(config);
        var user = new User("Test", "User", "test@email.com", new DateTime(1990, 1, 1));

        Assert.Throws<InvalidOperationException>(() => sut.GenerateJwtToken(user));
    }

    [Fact]
    public void CreatePasswordHash_WhenSecurityKeyIsEmpty_ShouldThrowInvalidOperationException()
    {
        var config = Substitute.For<IConfiguration>();
        config["Security:Key"].Returns(string.Empty);
        var sut = new PasswordManager(config);

        Assert.Throws<InvalidOperationException>(() => sut.CreatePasswordHash("password", out _));
    }

    [Fact]
    public void VerifyPassword_WhenSecurityKeyIsEmpty_ShouldThrowInvalidOperationException()
    {
        var config = Substitute.For<IConfiguration>();
        config["Security:Key"].Returns(string.Empty);
        var sut = new PasswordManager(config);

        Assert.Throws<InvalidOperationException>(() => sut.VerifyPassword("password", "hash"));
    }
}
