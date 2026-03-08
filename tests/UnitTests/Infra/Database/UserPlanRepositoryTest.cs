using Domain.Plan.Entities;
using Domain.UserPlan.Entities;
using Infra.Database.SqlServer.UserPlan.Repositories;

namespace UnitTests.Infra.Database;

public class UserPlanRepositoryTest : InfraTestBase
{
    private readonly UserPlanRepository _sut;

    public UserPlanRepositoryTest()
    {
        _sut = new UserPlanRepository(DbContext);
    }

    private async Task<Plan> SeedPlan(string name = "Plano Teste")
    {
        var plan = new Plan { Name = name, Price = 10m };
        await DbContext.Plans.AddAsync(plan);
        await DbContext.SaveChangesAsync();
        return plan;
    }

    private static UserPlan CreateUserPlan(string userId, int planId)
        => new UserPlan { UserId = userId, PlanId = planId };

    [Fact]
    public async Task CreateAsync_ShouldPersistUserPlanInDatabase()
    {
        // Arrange
        var plan = await SeedPlan();
        var userPlan = CreateUserPlan("user-1", plan.Id);

        // Act
        await _sut.CreateAsync(userPlan);

        // Assert
        var found = await DbContext.UserPlans.FindAsync(userPlan.Id);
        Assert.NotNull(found);
        Assert.Equal("user-1", found!.UserId);
        Assert.Equal(plan.Id, found.PlanId);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUserPlans()
    {
        // Arrange
        var plan = await SeedPlan();
        await _sut.CreateAsync(CreateUserPlan("user-A", plan.Id));
        await _sut.CreateAsync(CreateUserPlan("user-B", plan.Id));

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyList()
    {
        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ShouldReturnUserPlan()
    {
        // Arrange
        var plan = await SeedPlan();
        var userPlan = CreateUserPlan("user-99", plan.Id);
        await _sut.CreateAsync(userPlan);

        // Act
        var result = await _sut.GetByIdAsync(userPlan.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("user-99", result!.UserId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityNotFound_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetByIdAsync(9999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserId_WhenEntityExists_ShouldReturnUserPlan()
    {
        // Arrange
        var plan = await SeedPlan();
        await _sut.CreateAsync(CreateUserPlan("user-abc", plan.Id));

        // Act
        var result = await _sut.GetByUserId("user-abc");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("user-abc", result.UserId);
    }

    [Fact]
    public async Task GetByUserId_WhenEntityNotFound_ShouldThrowInvalidOperationException()
    {
        // Act & Assert — FirstAsync throws when no element found
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.GetByUserId("usuario-inexistente"));
    }

    [Fact]
    public void Delete_ShouldThrowNotImplementedException()
    {
        // Arrange
        var userPlan = new UserPlan { UserId = "user-x", PlanId = 1 };

        // Act & Assert
        Assert.Throws<NotImplementedException>(() => _sut.Delete(userPlan));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotImplementedException()
    {
        // Arrange
        var userPlan = new UserPlan { UserId = "user-x", PlanId = 1 };

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => _sut.UpdateAsync(userPlan));
    }
}
