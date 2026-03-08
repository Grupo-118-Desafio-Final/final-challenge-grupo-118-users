using NSubstitute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Application.Cache;
using FinalChallengeUsers.API.Controllers;
using PlanResponseDto = Domain.Plan.Dto.PlanResponseDto;
using PlanCreateRequestDto = Domain.Plan.Dto.PlanCreateRequestDto;
using IPlanManager = Domain.Plan.Ports.In.IPlanManager;

namespace UnitTests.Adapters.Plan;

public class PlanControllerTest
{
    private readonly IPlanManager _planManager;
    private readonly IDistributedCache _distributedCache;
    private readonly CacheService _cacheService;
    private readonly PlanController _sut;

    public PlanControllerTest()
    {
        _planManager = Substitute.For<IPlanManager>();
        _distributedCache = Substitute.For<IDistributedCache>();
        _distributedCache
            .GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((byte[])null!);
        _cacheService = new CacheService(_distributedCache);
        _sut = new PlanController(_planManager, _cacheService)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task GetById_WhenCacheMiss_ShouldCallManagerAndReturnOk()
    {
        // Arrange
        var planDto = new PlanResponseDto(1, "Plano Gold", 49.99m);
        _planManager.GetById(1).Returns(planDto);

        // Act
        var result = await _sut.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(planDto, okResult.Value);
        await _planManager.Received(1).GetById(1);
    }

    [Fact]
    public async Task GetById_WhenCacheHit_ShouldNotCallManager()
    {
        // Arrange
        var planDto = new PlanResponseDto(2, "Plano Silver", 29.99m);
        var serialized = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(planDto);
        _distributedCache
            .GetAsync("plan:2", Arg.Any<CancellationToken>())
            .Returns(serialized);

        // Act
        var result = await _sut.GetById(2);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        await _planManager.DidNotReceive().GetById(2);
    }

    [Fact]
    public async Task GetById_WhenManagerReturnsNull_ShouldReturnOkWithNull()
    {
        // Arrange
        _planManager.GetById(99).Returns((PlanResponseDto)null!);

        // Act
        var result = await _sut.GetById(99);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Null(okResult.Value);
    }

    [Fact]
    public async Task Create_ShouldCallManagerAndReturnCreated()
    {
        // Arrange
        var dto = new PlanCreateRequestDto { Name = "Plano Bronze", Price = 9.99m };

        // Act
        var result = await _sut.Create(dto);

        // Assert
        Assert.IsType<CreatedResult>(result);
        await _planManager.Received(1).CreateAsync(dto);
    }

    [Fact]
    public async Task Create_ShouldNotCallGetById()
    {
        // Arrange
        var dto = new PlanCreateRequestDto { Name = "Plano Teste", Price = 19.99m };

        // Act
        await _sut.Create(dto);

        // Assert
        await _planManager.DidNotReceive().GetById(Arg.Any<int>());
    }

    [Fact]
    public async Task GetPlanByUserId_WhenCacheMiss_ShouldCallManagerAndReturnOk()
    {
        // Arrange
        var userId = "user-123";
        var planDto = new PlanResponseDto(3, "Plano Premium", 99.99m);
        _planManager.GetPlanByUserId(userId).Returns(planDto);

        // Act
        var result = await _sut.GetPlanByUserId(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(planDto, okResult.Value);
        await _planManager.Received(1).GetPlanByUserId(userId);
    }

    [Fact]
    public async Task GetPlanByUserId_WhenCacheHit_ShouldNotCallManager()
    {
        // Arrange
        var userId = "user-456";
        var planDto = new PlanResponseDto(4, "Plano Basic", 5.99m);
        var serialized = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(planDto);
        _distributedCache
            .GetAsync($"user_plan:{userId}", Arg.Any<CancellationToken>())
            .Returns(serialized);

        // Act
        var result = await _sut.GetPlanByUserId(userId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        await _planManager.DidNotReceive().GetPlanByUserId(userId);
    }

    [Fact]
    public async Task GetAll_WhenCacheMiss_ShouldCallManagerAndReturnOk()
    {
        // Arrange
        var plans = new List<PlanResponseDto>
        {
            new PlanResponseDto(1, "Plano Gold", 49.99m),
            new PlanResponseDto(2, "Plano Silver", 29.99m)
        };
        _planManager.GetAll().Returns(plans);

        // Act
        var result = await _sut.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(plans, okResult.Value);
        await _planManager.Received(1).GetAll();
    }

    [Fact]
    public async Task GetAll_WhenCacheHit_ShouldNotCallManager()
    {
        // Arrange
        var plans = new List<PlanResponseDto> { new PlanResponseDto(1, "Plano Gold", 49.99m) };
        var serialized = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(plans);
        _distributedCache
            .GetAsync("plans:all", Arg.Any<CancellationToken>())
            .Returns(serialized);

        // Act
        var result = await _sut.GetAll();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        await _planManager.DidNotReceive().GetAll();
    }

    [Fact]
    public async Task GetAll_WhenManagerReturnsEmptyList_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        _planManager.GetAll().Returns(new List<PlanResponseDto>());

        // Act
        var result = await _sut.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var resultList = Assert.IsType<List<PlanResponseDto>>(okResult.Value);
        Assert.Empty(resultList);
    }
}
