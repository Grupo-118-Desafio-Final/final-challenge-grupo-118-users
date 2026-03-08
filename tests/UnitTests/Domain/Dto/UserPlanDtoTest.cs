using UserPlanEntity = Domain.UserPlan.Entities.UserPlan;
using UserPlanCreateRequest = Domain.UserPlan.Dto.UserPlanCreateRequest;
using UserPlanResponse = Domain.UserPlan.Dto.UserPlanResponse;
using UserPlanUpdateRequest = Domain.UserPlan.Dto.UserPlanUpdateRequest;

namespace UnitTests.Domain.Dto;

public class UserPlanDtoTest
{
    // --- UserPlanCreateRequest ---

    [Fact]
    public void UserPlanCreateRequest_ToDto_ShouldMapEntityToDto()
    {
        var entity = new UserPlanEntity { UserId = "user-1", PlanId = 5 };

        var dto = UserPlanCreateRequest.ToDto(entity);

        Assert.Equal("user-1", dto.UserId);
        Assert.Equal(5, dto.PlanId);
    }

    [Fact]
    public void UserPlanCreateRequest_ToEntity_ShouldMapDtoToEntity()
    {
        var dto = new UserPlanCreateRequest { UserId = "user-2", PlanId = 7 };

        var entity = UserPlanCreateRequest.ToEntity(dto);

        Assert.Equal("user-2", entity.UserId);
        Assert.Equal(7, entity.PlanId);
    }

    [Fact]
    public void UserPlanCreateRequest_ToEntity_ShouldPreserveUserIdAndPlanId()
    {
        var dto = new UserPlanCreateRequest { UserId = "abc-123", PlanId = 99 };

        var entity = UserPlanCreateRequest.ToEntity(dto);

        Assert.Equal("abc-123", entity.UserId);
        Assert.Equal(99, entity.PlanId);
    }

    // --- UserPlanResponse ---

    [Fact]
    public void UserPlanResponse_ToDto_ShouldMapAllFieldsFromEntity()
    {
        var entity = new UserPlanEntity { Id = 3, UserId = "user-3", PlanId = 9 };

        var dto = UserPlanResponse.ToDto(entity);

        Assert.Equal(3, dto.Id);
        Assert.Equal("user-3", dto.UserId);
        Assert.Equal(9, dto.PlanId);
    }

    [Fact]
    public void UserPlanResponse_ToEntity_ShouldMapAllFieldsFromDto()
    {
        var dto = new UserPlanResponse { Id = 4, UserId = "user-4", PlanId = 11 };

        var entity = UserPlanResponse.ToEntity(dto);

        Assert.Equal(4, entity.Id);
        Assert.Equal("user-4", entity.UserId);
        Assert.Equal(11, entity.PlanId);
    }

    [Fact]
    public void UserPlanResponse_RoundTrip_EntityToDtoAndBack_ShouldPreserveValues()
    {
        var original = new UserPlanEntity { Id = 10, UserId = "round-trip-user", PlanId = 42 };

        var dto = UserPlanResponse.ToDto(original);
        var restored = UserPlanResponse.ToEntity(dto);

        Assert.Equal(original.Id, restored.Id);
        Assert.Equal(original.UserId, restored.UserId);
        Assert.Equal(original.PlanId, restored.PlanId);
    }

    // --- UserPlanUpdateRequest ---

    [Fact]
    public void UserPlanUpdateRequest_ToDto_ShouldMapAllFieldsFromEntity()
    {
        var entity = new UserPlanEntity { Id = 5, UserId = "user-5", PlanId = 13 };

        var dto = UserPlanUpdateRequest.ToDto(entity);

        Assert.Equal(5, dto.Id);
        Assert.Equal("user-5", dto.UserId);
        Assert.Equal(13, dto.PlanId);
    }

    [Fact]
    public void UserPlanUpdateRequest_ToEntity_ShouldMapAllFieldsFromDto()
    {
        var dto = new UserPlanUpdateRequest { Id = 6, UserId = "user-6", PlanId = 15 };

        var entity = UserPlanUpdateRequest.ToEntity(dto);

        Assert.Equal(6, entity.Id);
        Assert.Equal("user-6", entity.UserId);
        Assert.Equal(15, entity.PlanId);
    }

    [Fact]
    public void UserPlanUpdateRequest_RoundTrip_EntityToDtoAndBack_ShouldPreserveValues()
    {
        var original = new UserPlanEntity { Id = 20, UserId = "update-round-trip", PlanId = 55 };

        var dto = UserPlanUpdateRequest.ToDto(original);
        var restored = UserPlanUpdateRequest.ToEntity(dto);

        Assert.Equal(original.Id, restored.Id);
        Assert.Equal(original.UserId, restored.UserId);
        Assert.Equal(original.PlanId, restored.PlanId);
    }
}
