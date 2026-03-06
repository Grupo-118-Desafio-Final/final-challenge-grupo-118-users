using NSubstitute;
using UserPlanManager = Application.UserPlan.UserPlanManager;
using UserPlanEntity = Domain.UserPlan.Entities.UserPlan;
using UserPlanCreateRequest = Domain.UserPlan.Dto.UserPlanCreateRequest;
using UserPlanUpdateRequest = Domain.UserPlan.Dto.UserPlanUpdateRequest;
using IUserPlanRepository = Domain.UserPlan.Ports.Out.IUserPlanRepository;

namespace UnitTests.Application.UserPlan;

public class UserPlanManagerTest
{
    private readonly IUserPlanRepository _userPlanRepository;
    private readonly UserPlanManager _sut;

    public UserPlanManagerTest()
    {
        _userPlanRepository = Substitute.For<IUserPlanRepository>();
        _sut = new UserPlanManager(_userPlanRepository);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallRepositoryCreateWithMappedEntity()
    {
        // Arrange
        var request = new UserPlanCreateRequest
        {
            UserId = "user-42",
            PlanId = 3
        };

        // Act
        await _sut.CreateAsync(request);

        // Assert
        await _userPlanRepository.Received(1).CreateAsync(Arg.Is<UserPlanEntity>(e =>
            e.UserId == request.UserId &&
            e.PlanId == request.PlanId
        ));
    }

    [Fact]
    public async Task DeleteAsync_ShouldGetByIdAndCallDelete()
    {
        // Arrange
        var entity = new UserPlanEntity { Id = 1, UserId = "user-10", PlanId = 2 };
        _userPlanRepository.GetByIdAsync(1).Returns(entity);

        // Act
        await _sut.DeleteAsync(1);

        // Assert
        await _userPlanRepository.Received(1).GetByIdAsync(1);
        _userPlanRepository.Received(1).Delete(entity);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedDtoList()
    {
        // Arrange
        var entities = new List<UserPlanEntity>
        {
            new() { Id = 1, UserId = "user-1", PlanId = 10 },
            new() { Id = 2, UserId = "user-2", PlanId = 20 }
        };
        _userPlanRepository.GetAllAsync().Returns(entities);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("user-1", result[0].UserId);
        Assert.Equal("user-2", result[1].UserId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ShouldReturnDto()
    {
        // Arrange
        var entity = new UserPlanEntity { Id = 5, UserId = "user-99", PlanId = 7 };
        _userPlanRepository.GetByIdAsync(5).Returns(entity);

        // Act
        var result = await _sut.GetByIdAsync(5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
        Assert.Equal("user-99", result.UserId);
        Assert.Equal(7, result.PlanId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityNotFound_ShouldReturnNull()
    {
        // Arrange
        _userPlanRepository.GetByIdAsync(99).Returns((UserPlanEntity)null!);

        // Act
        var result = await _sut.GetByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserId_WhenEntityExists_ShouldReturnDto()
    {
        // Arrange
        var entity = new UserPlanEntity { Id = 3, UserId = "user-abc", PlanId = 5 };
        _userPlanRepository.GetByUserId("user-abc").Returns(entity);

        // Act
        var result = await _sut.GetByUserId("user-abc");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("user-abc", result.UserId);
        Assert.Equal(5, result.PlanId);
    }

    [Fact]
    public async Task GetByUserId_WhenEntityNotFound_ShouldReturnNull()
    {
        // Arrange
        _userPlanRepository.GetByUserId("naoexiste").Returns((UserPlanEntity)null!);

        // Act
        var result = await _sut.GetByUserId("naoexiste");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityExists_ShouldUpdateFieldsAndCallRepository()
    {
        // Arrange
        var entity = new UserPlanEntity { Id = 1, UserId = "user-antigo", PlanId = 1 };
        var request = new UserPlanUpdateRequest { Id = 1, UserId = "user-novo", PlanId = 9 };
        _userPlanRepository.GetByIdAsync(1).Returns(entity);

        // Act
        await _sut.UpdateAsync(request);

        // Assert
        Assert.Equal("user-novo", entity.UserId);
        Assert.Equal(9, entity.PlanId);
        await _userPlanRepository.Received(1).UpdateAsync(entity);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityNotFound_ShouldCallUpdateWithNull()
    {
        // Arrange
        _userPlanRepository.GetByIdAsync(99).Returns((UserPlanEntity)null!);
        var request = new UserPlanUpdateRequest { Id = 99, UserId = "qualquer", PlanId = 1 };

        // Act
        await _sut.UpdateAsync(request);

        // Assert
        await _userPlanRepository.Received(1).UpdateAsync(null!);
    }
}
