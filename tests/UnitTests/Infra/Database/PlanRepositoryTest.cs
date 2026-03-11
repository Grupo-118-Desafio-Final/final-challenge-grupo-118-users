using Domain.Plan.Entities;
using Domain.Plan.ValueObjects;
using Infra.Database.SqlServer.Plan.Repositories;

namespace UnitTests.Infra.Database;

public class PlanRepositoryTest : InfraTestBase
{
    private readonly PlanRepository _sut;

    public PlanRepositoryTest()
    {
        _sut = new PlanRepository(DbContext);
    }

    private static Plan CreatePlan(string name = "Plano Teste", decimal price = 29.99m)
        => new Plan
        {
            Name = name,
            Price = price,
            ImageQuality = ImageQualityEnum.Hd,
            MaxSizeInMegaBytes = "200",
            MaxDurationInSeconds = "20",
            DesiredFrames = "1"
        };

    [Fact]
    public async Task CreateAsync_ShouldPersistPlanInDatabase()
    {
        // Arrange
        var plan = CreatePlan("Plano Gold", 99.99m);

        // Act
        await _sut.CreateAsync(plan);

        // Assert
        var found = await DbContext.Plans.FindAsync(plan.Id);
        Assert.NotNull(found);
        Assert.Equal("Plano Gold", found!.Name);
        Assert.Equal(99.99m, found.Price);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllPlans()
    {
        // Arrange
        await _sut.CreateAsync(CreatePlan("Plano A", 10m));
        await _sut.CreateAsync(CreatePlan("Plano B", 20m));

        // Act
        var result = await _sut.GetAll();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAll_WhenEmpty_ShouldReturnEmptyList()
    {
        // Act
        var result = await _sut.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPlanExists_ShouldReturnPlan()
    {
        // Arrange
        var plan = CreatePlan("Plano Silver");
        await _sut.CreateAsync(plan);

        // Act
        var result = await _sut.GetByIdAsync(plan.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Plano Silver", result!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPlanNotFound_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetByIdAsync(9999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_WhenPlanExists_ShouldReturnPlan()
    {
        // Arrange
        var plan = CreatePlan("Plano Premium");
        await _sut.CreateAsync(plan);

        // Act
        var result = await _sut.GetByNameAsync("Plano Premium");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Plano Premium", result!.Name);
    }

    [Fact]
    public async Task GetByNameAsync_WhenPlanNotFound_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetByNameAsync("Plano Inexistente");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Delete_ShouldThrowNotImplementedException()
    {
        // Arrange
        var plan = CreatePlan();

        // Act & Assert
        Assert.Throws<NotImplementedException>(() => _sut.Delete(plan));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotImplementedException()
    {
        // Arrange
        var plan = CreatePlan();

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => _sut.UpdateAsync(plan));
    }
}
