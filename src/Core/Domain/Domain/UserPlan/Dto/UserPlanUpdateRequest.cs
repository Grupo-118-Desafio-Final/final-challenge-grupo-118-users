namespace Domain.UserPlan.Dto;

public class UserPlanUpdateRequest
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int PlanId { get; set; }

    public static UserPlanUpdateRequest ToDto(Entities.UserPlan userPlan)
    {
        return new UserPlanUpdateRequest
        {
            Id = userPlan.Id,
            UserId = userPlan.UserId,
            PlanId = userPlan.PlanId
        };
    }

    public static Entities.UserPlan ToEntity(UserPlanUpdateRequest userPlanUpdateRequest)
    {
        return new Entities.UserPlan
        {
            Id = userPlanUpdateRequest.Id,
            UserId = userPlanUpdateRequest.UserId,
            PlanId= userPlanUpdateRequest.PlanId
        };
    }
}
