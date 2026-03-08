using NSubstitute;
using Microsoft.Extensions.Caching.Distributed;
using Application.Cache;
using System.Text;
using System.Text.Json;

namespace UnitTests.Application.Cache;

public class CacheServiceTest
{
    private readonly IDistributedCache _distributedCache;
    private readonly CacheService _sut;

    public CacheServiceTest()
    {
        _distributedCache = Substitute.For<IDistributedCache>();
        _sut = new CacheService(_distributedCache);
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenCacheMiss_ShouldCallFactory()
    {
        // Arrange
        _distributedCache
            .GetAsync("test-key", Arg.Any<CancellationToken>())
            .Returns((byte[])null!);
        var factoryCalled = false;

        // Act
        var result = await _sut.GetOrCreateAsync(
            "test-key",
            async () =>
            {
                factoryCalled = true;
                return "factory-value";
            },
            TimeSpan.FromMinutes(10));

        // Assert
        Assert.True(factoryCalled);
        Assert.Equal("factory-value", result);
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenCacheMiss_ShouldStoreResultInCache()
    {
        // Arrange
        _distributedCache
            .GetAsync("store-key", Arg.Any<CancellationToken>())
            .Returns((byte[])null!);

        // Act
        await _sut.GetOrCreateAsync(
            "store-key",
            async () => "stored-value",
            TimeSpan.FromMinutes(5));

        // Assert
        await _distributedCache.Received(1).SetAsync(
            "store-key",
            Arg.Any<byte[]>(),
            Arg.Any<DistributedCacheEntryOptions>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenCacheHit_ShouldReturnCachedValue()
    {
        // Arrange
        var cachedValue = "cached-value";
        var jsonBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cachedValue));
        _distributedCache
            .GetAsync("hit-key", Arg.Any<CancellationToken>())
            .Returns(jsonBytes);

        // Act
        var result = await _sut.GetOrCreateAsync(
            "hit-key",
            async () => "new-value",
            TimeSpan.FromMinutes(10));

        // Assert
        Assert.Equal(cachedValue, result);
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenCacheHit_ShouldNotCallFactory()
    {
        // Arrange
        var cachedDto = new TestDto { Value = 42, Label = "cached" };
        var jsonBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cachedDto));
        _distributedCache
            .GetAsync("no-factory-key", Arg.Any<CancellationToken>())
            .Returns(jsonBytes);
        var factoryCalled = false;

        // Act
        await _sut.GetOrCreateAsync(
            "no-factory-key",
            async () =>
            {
                factoryCalled = true;
                return new TestDto { Value = 0, Label = "new" };
            },
            TimeSpan.FromMinutes(10));

        // Assert
        Assert.False(factoryCalled);
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenCacheHit_ShouldNotCallSetAsync()
    {
        // Arrange
        var jsonBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize("value"));
        _distributedCache
            .GetAsync("no-set-key", Arg.Any<CancellationToken>())
            .Returns(jsonBytes);

        // Act
        await _sut.GetOrCreateAsync(
            "no-set-key",
            async () => "new-value",
            TimeSpan.FromMinutes(10));

        // Assert
        await _distributedCache.DidNotReceive().SetAsync(
            Arg.Any<string>(),
            Arg.Any<byte[]>(),
            Arg.Any<DistributedCacheEntryOptions>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOrCreateAsync_WithComplexType_ShouldDeserializeCorrectly()
    {
        // Arrange
        var original = new TestDto { Value = 99, Label = "complex" };
        var jsonBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(original));
        _distributedCache
            .GetAsync("complex-key", Arg.Any<CancellationToken>())
            .Returns(jsonBytes);

        // Act
        var result = await _sut.GetOrCreateAsync(
            "complex-key",
            async () => new TestDto { Value = 0, Label = "" },
            TimeSpan.FromMinutes(10));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(99, result!.Value);
        Assert.Equal("complex", result.Label);
    }

    private class TestDto
    {
        public int Value { get; set; }
        public string Label { get; set; } = string.Empty;
    }
}
