using Domain.ApiKey.Entities;
using Domain.Plan.Entities;
using Domain.Users.Entities;
using Domain.UserPlan.Entities;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Infra.Database;

public class AppDbContextTest : InfraTestBase
{
    [Fact]
    public void AppDbContext_ShouldExposeUsersDbSet()
    {
        Assert.NotNull(DbContext.Users);
    }

    [Fact]
    public void AppDbContext_ShouldExposePlansDbSet()
    {
        Assert.NotNull(DbContext.Plans);
    }

    [Fact]
    public void AppDbContext_ShouldExposeUserPlansDbSet()
    {
        Assert.NotNull(DbContext.UserPlans);
    }

    [Fact]
    public void AppDbContext_ShouldExposeApiKeysDbSet()
    {
        Assert.NotNull(DbContext.ApiKeys);
    }

    [Fact]
    public void AppDbContext_Model_ShouldContainUserEntity()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(User));
        Assert.NotNull(entityType);
    }

    [Fact]
    public void AppDbContext_Model_ShouldContainPlanEntity()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(Plan));
        Assert.NotNull(entityType);
    }

    [Fact]
    public void AppDbContext_Model_ShouldContainUserPlanEntity()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(UserPlan));
        Assert.NotNull(entityType);
    }

    [Fact]
    public void AppDbContext_Model_ShouldContainApiKeyEntity()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(ApiKey));
        Assert.NotNull(entityType);
    }

    [Fact]
    public async Task AppDbContext_ShouldSaveAndRetrieveData()
    {
        var user = new User("Test", "User", "test@email.com", new DateTime(1990, 1, 1));
        user.SetPassword("hash-senha");
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();

        var retrieved = await DbContext.Users.FindAsync(user.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("Test", retrieved!.Name);
    }
}
