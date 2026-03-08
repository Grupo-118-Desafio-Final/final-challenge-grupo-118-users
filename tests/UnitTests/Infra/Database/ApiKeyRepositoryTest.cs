using Domain.ApiKey.Entities;
using Infra.Database.SqlServer.ApiKey.Repositories;

namespace UnitTests.Infra.Database;

public class ApiKeyRepositoryTest : InfraTestBase
{
    private readonly ApiKeyRepository _sut;

    public ApiKeyRepositoryTest()
    {
        _sut = new ApiKeyRepository(DbContext);
    }

    [Fact]
    public async Task GetByKeyAsync_WhenKeyExistsAndIsActive_ShouldReturnApiKey()
    {
        // Arrange
        var apiKey = new ApiKey("minha-chave-valida", isActive: true);
        await DbContext.ApiKeys.AddAsync(apiKey);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetByKeyAsync("minha-chave-valida");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("minha-chave-valida", result!.Key);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetByKeyAsync_WhenKeyDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetByKeyAsync("chave-inexistente");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByKeyAsync_WhenKeyExistsButIsInactive_ShouldReturnNull()
    {
        // Arrange
        var apiKey = new ApiKey("chave-inativa", isActive: false);
        await DbContext.ApiKeys.AddAsync(apiKey);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetByKeyAsync("chave-inativa");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByKeyAsync_WithMultipleKeys_ShouldReturnCorrectOne()
    {
        // Arrange
        await DbContext.ApiKeys.AddRangeAsync(
            new ApiKey("chave-A"),
            new ApiKey("chave-B"),
            new ApiKey("chave-C", isActive: false));
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetByKeyAsync("chave-B");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("chave-B", result!.Key);
    }
}
