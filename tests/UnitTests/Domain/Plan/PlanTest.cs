using PlanEntity = Domain.Plan.Entities.Plan;
using UserPlanEntity = Domain.UserPlan.Entities.UserPlan;
using ImageQuality = Domain.Plan.ValueObjects.ImageQualityEnum;
using BaseEntity = Domain.Base.Entities.BaseEntity;

namespace UnitTests.Domain.Plan;

public class PlanTest
{
    [Fact]
    public void Plan_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var plan = new PlanEntity();

        // Assert
        Assert.Equal(ImageQuality.Hd, plan.ImageQuality);
        Assert.Equal("200", plan.MaxSizeInMegaBytes);
        Assert.Equal("20", plan.MaxDurationInSeconds);
        Assert.Equal("1", plan.Threads);
        Assert.NotNull(plan.UsersPlans);
        Assert.Empty(plan.UsersPlans);
    }

    [Fact]
    public void Plan_SetName_ShouldReturnCorrectValue()
    {
        // Arrange
        var plan = new PlanEntity();

        // Act
        plan.Name = "Plano Básico";

        // Assert
        Assert.Equal("Plano Básico", plan.Name);
    }

    [Fact]
    public void Plan_SetPrice_ShouldReturnCorrectValue()
    {
        // Arrange
        var plan = new PlanEntity();

        // Act
        plan.Price = 29.99m;

        // Assert
        Assert.Equal(29.99m, plan.Price);
    }

    [Theory]
    [InlineData(ImageQuality.Hd)]
    [InlineData(ImageQuality.FullHd)]
    [InlineData(ImageQuality.UltraHd)]
    [InlineData(ImageQuality.FourK)]
    [InlineData(ImageQuality.EightK)]
    public void Plan_SetImageQuality_ShouldReturnCorrectValue(ImageQuality quality)
    {
        // Arrange
        var plan = new PlanEntity();

        // Act
        plan.ImageQuality = quality;

        // Assert
        Assert.Equal(quality, plan.ImageQuality);
    }

    [Fact]
    public void Plan_SetMaxSizeInMegaBytes_ShouldReturnCorrectValue()
    {
        // Arrange
        var plan = new PlanEntity();

        // Act
        plan.MaxSizeInMegaBytes = "500";

        // Assert
        Assert.Equal("500", plan.MaxSizeInMegaBytes);
    }

    [Fact]
    public void Plan_SetMaxDurationInSeconds_ShouldReturnCorrectValue()
    {
        // Arrange
        var plan = new PlanEntity();

        // Act
        plan.MaxDurationInSeconds = "60";

        // Assert
        Assert.Equal("60", plan.MaxDurationInSeconds);
    }

    [Fact]
    public void Plan_SetThreads_ShouldReturnCorrectValue()
    {
        // Arrange
        var plan = new PlanEntity();

        // Act
        plan.Threads = "4";

        // Assert
        Assert.Equal("4", plan.Threads);
    }

    [Fact]
    public void Plan_SetId_ShouldReturnCorrectValue()
    {
        // Arrange
        var plan = new PlanEntity();

        // Act
        plan.Id = 42;

        // Assert
        Assert.Equal(42, plan.Id);
    }

    [Fact]
    public void Plan_SetCreatedAt_ShouldReturnCorrectValue()
    {
        // Arrange
        var plan = new PlanEntity();
        var date = new DateTime(2025, 1, 15);

        // Act
        plan.CreatedAt = date;

        // Assert
        Assert.Equal(date, plan.CreatedAt);
    }

    [Fact]
    public void Plan_SetUpdatedAt_ShouldReturnCorrectValue()
    {
        // Arrange
        var plan = new PlanEntity();
        var date = new DateTime(2025, 6, 20);

        // Act
        plan.UpdatedAt = date;

        // Assert
        Assert.Equal(date, plan.UpdatedAt);
    }

    [Fact]
    public void Plan_AddUserPlan_ShouldIncreaseCollectionCount()
    {
        // Arrange
        var plan = new PlanEntity();
        var userPlan = new UserPlanEntity();

        // Act
        plan.UsersPlans.Add(userPlan);

        // Assert
        Assert.Single(plan.UsersPlans);
    }

    [Fact]
    public void Plan_SetUsersPlans_ShouldReturnCorrectCollection()
    {
        // Arrange
        var userPlan1 = new UserPlanEntity();
        var userPlan2 = new UserPlanEntity();

        // Act
        var plan = new PlanEntity
        {
            UsersPlans = new List<UserPlanEntity> { userPlan1, userPlan2 }
        };

        // Assert
        Assert.Equal(2, plan.UsersPlans.Count);
        Assert.Contains(userPlan1, plan.UsersPlans);
        Assert.Contains(userPlan2, plan.UsersPlans);
    }

    [Fact]
    public void Plan_InheritsFromBaseEntity_ShouldHaveBaseEntityProperties()
    {
        // Arrange & Act
        var plan = new PlanEntity();

        // Assert
        Assert.IsAssignableFrom<BaseEntity>(plan);
    }
}
