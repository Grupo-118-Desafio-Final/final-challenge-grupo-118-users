using PlanEntity = Domain.Plan.Entities.Plan;
using UserPlanEntity = Domain.UserPlan.Entities.UserPlan;
using BaseEntity = Domain.Base.Entities.BaseEntity;

namespace UnitTests.Domain.UserPlan;

public class UserPlanTest
{
    [Fact]
    public void UserPlan_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var userPlan = new UserPlanEntity();

        // Assert
        Assert.Equal(0, userPlan.Id);
        Assert.Equal(0, userPlan.PlanId);
    }

    [Fact]
    public void UserPlan_SetUserId_ShouldReturnCorrectValue()
    {
        // Arrange
        var userPlan = new UserPlanEntity();

        // Act
        userPlan.UserId = "user-123";

        // Assert
        Assert.Equal("user-123", userPlan.UserId);
    }

    [Fact]
    public void UserPlan_SetPlanId_ShouldReturnCorrectValue()
    {
        // Arrange
        var userPlan = new UserPlanEntity();

        // Act
        userPlan.PlanId = 5;

        // Assert
        Assert.Equal(5, userPlan.PlanId);
    }

    [Fact]
    public void UserPlan_SetPlan_ShouldReturnCorrectValue()
    {
        // Arrange
        var plan = new PlanEntity { Id = 1, Name = "Plano Gold", Price = 49.99m };
        var userPlan = new UserPlanEntity();

        // Act
        userPlan.Plan = plan;

        // Assert
        Assert.Equal(plan, userPlan.Plan);
        Assert.Equal("Plano Gold", userPlan.Plan.Name);
    }

    [Fact]
    public void UserPlan_SetId_ShouldReturnCorrectValue()
    {
        // Arrange
        var userPlan = new UserPlanEntity();

        // Act
        userPlan.Id = 10;

        // Assert
        Assert.Equal(10, userPlan.Id);
    }

    [Fact]
    public void UserPlan_SetCreatedAt_ShouldReturnCorrectValue()
    {
        // Arrange
        var userPlan = new UserPlanEntity();
        var date = new DateTime(2025, 3, 1);

        // Act
        userPlan.CreatedAt = date;

        // Assert
        Assert.Equal(date, userPlan.CreatedAt);
    }

    [Fact]
    public void UserPlan_SetUpdatedAt_ShouldReturnCorrectValue()
    {
        // Arrange
        var userPlan = new UserPlanEntity();
        var date = new DateTime(2025, 6, 15);

        // Act
        userPlan.UpdatedAt = date;

        // Assert
        Assert.Equal(date, userPlan.UpdatedAt);
    }

    [Fact]
    public void UserPlan_InheritsFromBaseEntity_ShouldHaveBaseEntityProperties()
    {
        // Arrange & Act
        var userPlan = new UserPlanEntity();

        // Assert
        Assert.IsType<BaseEntity>(userPlan, exactMatch: false);
    }

    [Fact]
    public void UserPlan_PlanReference_ShouldLinkCorrectlyToPlan()
    {
        // Arrange
        var plan = new PlanEntity { Id = 3, Price = 99.90m };

        // Act
        var userPlan = new UserPlanEntity { PlanId = 3, Plan = plan };

        // Assert
        Assert.Equal(userPlan.PlanId, userPlan.Plan.Id);
    }
}
