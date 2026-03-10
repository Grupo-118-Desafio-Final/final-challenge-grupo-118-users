using Microsoft.Extensions.Logging;
using NSubstitute;
using PlanManager = Application.Plans.PlanManager;
using PlanEntity = Domain.Plan.Entities.Plan;
using PlanCreateRequestDto = Domain.Plan.Dto.PlanCreateRequestDto;
using PlanUpdateRequestDto = Domain.Plan.Dto.PlanUpdateRequestDto;
using ImageQualityEnum = Domain.Plan.ValueObjects.ImageQualityEnum;
using IPlanRepository = Domain.Plan.Ports.Out.IPlanRepository;
using UserPlanResponse = Domain.UserPlan.Dto.UserPlanResponse;
using IUserPlanManager = Domain.UserPlan.Ports.In.IUserPlanManager;

namespace UnitTests.Application.Plan;

public class PlanManagerTest
{
    private readonly IPlanRepository _planRepository;
    private readonly IUserPlanManager _userPlanManager;
    private readonly PlanManager _sut;

    public PlanManagerTest()
    {
        _planRepository = Substitute.For<IPlanRepository>();
        _userPlanManager = Substitute.For<IUserPlanManager>();
        _sut = new PlanManager(Substitute.For<ILogger<PlanManager>>(), _planRepository, _userPlanManager);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallRepositoryCreateWithMappedEntity()
    {
        // Arrange
        var dto = new PlanCreateRequestDto
        {
            Name = "Plano Básico",
            Price = 29.99m,
            ImageQuality = ImageQualityEnum.FullHd,
            MaxSizeInMegaBytes = "300",
            MaxDurationInSeconds = "30",
            Threads = "2"
        };

        // Act
        await _sut.CreateAsync(dto);

        // Assert
        await _planRepository.Received(1).CreateAsync(Arg.Is<PlanEntity>(p =>
            p.Name == dto.Name &&
            p.Price == dto.Price &&
            p.ImageQuality == dto.ImageQuality &&
            p.MaxSizeInMegaBytes == dto.MaxSizeInMegaBytes &&
            p.MaxDurationInSeconds == dto.MaxDurationInSeconds &&
            p.Threads == dto.Threads
        ));
    }

    [Fact]
    public async Task DeleteAsync_ShouldGetByIdAndCallDelete()
    {
        // Arrange
        var plan = new PlanEntity { Id = 1, Name = "Plano A" };
        _planRepository.GetByIdAsync(1).Returns(plan);

        // Act
        await _sut.DeleteAsync(1);

        // Assert
        await _planRepository.Received(1).GetByIdAsync(1);
        _planRepository.Received(1).Delete(plan);
    }

    [Fact]
    public async Task GetAll_ShouldReturnMappedDtoList()
    {
        // Arrange
        var plans = new List<PlanEntity>
        {
            new() { Id = 1, Name = "Plano A", Price = 10m },
            new() { Id = 2, Name = "Plano B", Price = 20m }
        };
        _planRepository.GetAll().Returns(plans);

        // Act
        var result = await _sut.GetAll();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Plano A", result[0].Name);
        Assert.Equal("Plano B", result[1].Name);
    }

    [Fact]
    public async Task GetById_WhenPlanExists_ShouldReturnDto()
    {
        // Arrange
        var plan = new PlanEntity { Id = 1, Name = "Plano Gold", Price = 99m };
        _planRepository.GetByIdAsync(1).Returns(plan);

        // Act
        var result = await _sut.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Plano Gold", result.Name);
        Assert.Equal(99m, result.Price);
    }

    [Fact]
    public async Task GetById_WhenPlanNotFound_ShouldReturnNull()
    {
        // Arrange
        _planRepository.GetByIdAsync(99).Returns((PlanEntity)null!);

        // Act
        var result = await _sut.GetById(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_WhenPlanExists_ShouldReturnDto()
    {
        // Arrange
        var plan = new PlanEntity { Id = 2, Name = "Plano Silver", Price = 49m };
        _planRepository.GetByNameAsync("Plano Silver").Returns(plan);

        // Act
        var result = await _sut.GetByNameAsync("Plano Silver");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Plano Silver", result.Name);
    }

    [Fact]
    public async Task GetByNameAsync_WhenPlanNotFound_ShouldReturnNull()
    {
        // Arrange
        _planRepository.GetByNameAsync("Inexistente").Returns((PlanEntity)null!);

        // Act
        var result = await _sut.GetByNameAsync("Inexistente");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPlanByUserId_ShouldReturnPlanOfUser()
    {
        // Arrange
        var userPlanResponse = new UserPlanResponse { PlanId = 3, UserId = "user-42" };
        var plan = new PlanEntity { Id = 3, Name = "Plano Premium", Price = 199m };
        _userPlanManager.GetByUserId("user-42").Returns(userPlanResponse);
        _planRepository.GetByIdAsync(3).Returns(plan);

        // Act
        var result = await _sut.GetPlanByUserId("user-42");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Id);
        Assert.Equal("Plano Premium", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenPlanExists_ShouldUpdateNameAndPrice()
    {
        // Arrange
        var existing = new PlanEntity { Id = 1, Name = "Antigo", Price = 10m };
        var dto = new PlanUpdateRequestDto(1, "Novo Nome", 99m);
        _planRepository.GetByIdAsync(1).Returns(existing);

        // Act
        await _sut.UpdateAsync(dto);

        // Assert
        Assert.Equal("Novo Nome", existing.Name);
        Assert.Equal(99m, existing.Price);
        await _planRepository.Received(1).UpdateAsync(existing);
    }

    [Fact]
    public async Task UpdateAsync_WhenPlanNotFound_ShouldNotCallUpdateRepository()
    {
        // Arrange
        _planRepository.GetByIdAsync(99).Returns((PlanEntity)null!);
        var dto = new PlanUpdateRequestDto(99, "Qualquer", 0m);

        // Act
        await _sut.UpdateAsync(dto);

        // Assert
        await _planRepository.DidNotReceive().UpdateAsync(Arg.Any<PlanEntity>());
    }
}
