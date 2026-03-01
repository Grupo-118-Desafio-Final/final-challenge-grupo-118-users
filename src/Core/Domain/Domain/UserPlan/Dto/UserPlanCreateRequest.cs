namespace Domain.UserPlan.Dto;

public class UserPlanCreateRequest
{
    public string UserId { get; set; }
    public int PlanId { get; set; }

    public static UserPlanCreateRequest ToDto(Entities.UserPlan userPlan)
    {
        return new UserPlanCreateRequest
        {
            UserId = userPlan.UserId,
            PlanId = userPlan.PlanId
        };
    }

    public static Entities.UserPlan ToEntity(UserPlanCreateRequest userPlanCreateRequest)
    {
        return new Entities.UserPlan
        {
            UserId = userPlanCreateRequest.UserId,
            PlanId = userPlanCreateRequest.PlanId
        };
    }
}
