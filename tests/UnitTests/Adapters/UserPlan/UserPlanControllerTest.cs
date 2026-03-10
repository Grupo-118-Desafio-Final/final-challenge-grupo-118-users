using NSubstitute;
using Microsoft.AspNetCore.Mvc;
using FinalChallengeUsers.API.Controllers;
using IUserPlanManager = Domain.UserPlan.Ports.In.IUserPlanManager;
using UserPlanResponse = Domain.UserPlan.Dto.UserPlanResponse;
using UserPlanCreateRequest = Domain.UserPlan.Dto.UserPlanCreateRequest;
using UserPlanUpdateRequest = Domain.UserPlan.Dto.UserPlanUpdateRequest;

namespace UnitTests.Adapters.UserPlan;

public class UserPlanControllerTest
{
    private readonly IUserPlanManager _userPlanManager;
    private readonly UserPlanController _sut;

    public UserPlanControllerTest()
    {
        _userPlanManager = Substitute.For<IUserPlanManager>();
        _sut = new UserPlanController(_userPlanManager);
    }

    [Fact]
    public async Task Create_ShouldCallManagerAndReturnCreated()
    {
        // Arrange
        var request = new UserPlanCreateRequest { UserId = "user-1", PlanId = 1 };

        // Act
        var result = await _sut.Create(request);

        // Assert
        Assert.IsType<CreatedResult>(result);
        await _userPlanManager.Received(1).CreateAsync(request);
    }

    [Fact]
    public async Task Update_ShouldCallManagerAndReturnNoContent()
    {
        // Arrange
        var request = new UserPlanUpdateRequest { Id = 1, UserId = "user-1", PlanId = 2 };

        // Act
        var result = await _sut.Update(request);

        // Assert
        Assert.IsType<NoContentResult>(result);
        await _userPlanManager.Received(1).UpdateAsync(request);
    }

    [Fact]
    public async Task GetById_WhenUserPlanExists_ShouldReturnOk()
    {
        // Arrange
        var userPlan = new UserPlanResponse { Id = 1, UserId = "user-1", PlanId = 2 };
        _userPlanManager.GetByIdAsync(1).Returns(userPlan);

        // Act
        var result = await _sut.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(userPlan, okResult.Value);
    }

    [Fact]
    public async Task GetById_WhenUserPlanNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _userPlanManager.GetByIdAsync(99).Returns((UserPlanResponse)null!);

        // Act
        var result = await _sut.GetById(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetByUserId_WhenUserPlanExists_ShouldReturnOk()
    {
        // Arrange
        var userPlan = new UserPlanResponse { Id = 1, UserId = "user-1", PlanId = 2 };
        _userPlanManager.GetByUserId("user-1").Returns(userPlan);

        // Act
        var result = await _sut.GetByUserId("user-1");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(userPlan, okResult.Value);
    }

    [Fact]
    public async Task GetByUserId_WhenUserPlanNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _userPlanManager.GetByUserId("no-plan-user").Returns((UserPlanResponse)null!);

        // Act
        var result = await _sut.GetByUserId("no-plan-user");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithList()
    {
        // Arrange
        var userPlans = new List<UserPlanResponse>
        {
            new() { Id = 1, UserId = "user-1", PlanId = 1 },
            new() { Id = 2, UserId = "user-2", PlanId = 2 }
        };
        _userPlanManager.GetAllAsync().Returns(userPlans);

        // Act
        var result = await _sut.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(userPlans, okResult.Value);
    }

    [Fact]
    public async Task GetAll_WhenManagerReturnsEmptyList_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        _userPlanManager.GetAllAsync().Returns(new List<UserPlanResponse>());

        // Act
        var result = await _sut.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var resultList = Assert.IsType<List<UserPlanResponse>>(okResult.Value);
        Assert.Empty(resultList);
    }

    [Fact]
    public async Task Delete_ShouldCallManagerAndReturnNoContent()
    {
        // Arrange & Act
        var result = await _sut.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        await _userPlanManager.Received(1).DeleteAsync(1);
    }
}
