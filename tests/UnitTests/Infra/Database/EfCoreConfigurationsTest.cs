using Domain.ApiKey.Entities;
using Domain.Plan.Entities;
using Domain.Users.Entities;
using Domain.UserPlan.Entities;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Infra.Database;

/// <summary>
/// Verifica que as configurações EF Core (ApiKeyConfiguration, PlanConfiguration,
/// UserConfiguration, UserPlanConfiguration) são aplicadas corretamente ao modelo.
/// </summary>
public class EfCoreConfigurationsTest : InfraTestBase
{
    // --- ApiKeyConfiguration ---

    [Fact]
    public void ApiKeyConfiguration_ShouldRegisterApiKeyEntityInModel()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(ApiKey));
        Assert.NotNull(entityType);
    }

    [Fact]
    public void ApiKeyConfiguration_IdShouldBePrimaryKey()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(ApiKey))!;
        var key = entityType.FindPrimaryKey();
        Assert.NotNull(key);
        Assert.Contains(key!.Properties, p => p.Name == nameof(ApiKey.Id));
    }

    [Fact]
    public void ApiKeyConfiguration_KeyPropertyShouldBeRequired()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(ApiKey))!;
        var property = entityType.FindProperty(nameof(ApiKey.Key));
        Assert.NotNull(property);
        Assert.False(property!.IsNullable);
    }

    [Fact]
    public void ApiKeyConfiguration_IsActivePropertyShouldBeRequired()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(ApiKey))!;
        var property = entityType.FindProperty(nameof(ApiKey.IsActive));
        Assert.NotNull(property);
        Assert.False(property!.IsNullable);
    }

    // --- PlanConfiguration ---

    [Fact]
    public void PlanConfiguration_ShouldRegisterPlanEntityInModel()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(Plan));
        Assert.NotNull(entityType);
    }

    [Fact]
    public void PlanConfiguration_IdShouldBePrimaryKey()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(Plan))!;
        var key = entityType.FindPrimaryKey();
        Assert.NotNull(key);
        Assert.Contains(key!.Properties, p => p.Name == nameof(Plan.Id));
    }

    [Fact]
    public void PlanConfiguration_NamePropertyShouldBeRequired()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(Plan))!;
        var property = entityType.FindProperty(nameof(Plan.Name));
        Assert.NotNull(property);
        Assert.False(property!.IsNullable);
    }

    [Fact]
    public void PlanConfiguration_PricePropertyShouldBeRequired()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(Plan))!;
        var property = entityType.FindProperty(nameof(Plan.Price));
        Assert.NotNull(property);
        Assert.False(property!.IsNullable);
    }

    // --- UserConfiguration ---

    [Fact]
    public void UserConfiguration_ShouldRegisterUserEntityInModel()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(User));
        Assert.NotNull(entityType);
    }

    [Fact]
    public void UserConfiguration_IdShouldBePrimaryKey()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(User))!;
        var key = entityType.FindPrimaryKey();
        Assert.NotNull(key);
        Assert.Contains(key!.Properties, p => p.Name == nameof(User.Id));
    }

    [Fact]
    public void UserConfiguration_NamePropertyShouldBeRequired()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(User))!;
        var property = entityType.FindProperty(nameof(User.Name));
        Assert.NotNull(property);
        Assert.False(property!.IsNullable);
    }

    [Fact]
    public void UserConfiguration_EmailPropertyShouldBeRequired()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(User))!;
        var property = entityType.FindProperty(nameof(User.Email));
        Assert.NotNull(property);
        Assert.False(property!.IsNullable);
    }

    [Fact]
    public void UserConfiguration_LastNamePropertyShouldBeRequired()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(User))!;
        var property = entityType.FindProperty(nameof(User.LastName));
        Assert.NotNull(property);
        Assert.False(property!.IsNullable);
    }

    // --- UserPlanConfiguration ---

    [Fact]
    public void UserPlanConfiguration_ShouldRegisterUserPlanEntityInModel()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(UserPlan));
        Assert.NotNull(entityType);
    }

    [Fact]
    public void UserPlanConfiguration_IdShouldBePrimaryKey()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(UserPlan))!;
        var key = entityType.FindPrimaryKey();
        Assert.NotNull(key);
        Assert.Contains(key!.Properties, p => p.Name == nameof(UserPlan.Id));
    }

    [Fact]
    public void UserPlanConfiguration_UserIdPropertyShouldBeRequired()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(UserPlan))!;
        var property = entityType.FindProperty(nameof(UserPlan.UserId));
        Assert.NotNull(property);
        Assert.False(property!.IsNullable);
    }

    [Fact]
    public void UserPlanConfiguration_PlanRelationshipShouldExist()
    {
        var entityType = DbContext.Model.FindEntityType(typeof(UserPlan))!;
        var foreignKey = entityType.GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Plan));
        Assert.NotNull(foreignKey);
    }
}
