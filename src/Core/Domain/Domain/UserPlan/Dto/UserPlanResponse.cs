namespace Domain.UserPlan.Dto;

public class UserPlanResponse
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int PlanId { get; set; }

    public static UserPlanResponse ToDto(Entities.UserPlan userPlan)
    {
        return new UserPlanResponse
        {
            Id = userPlan.Id,
            UserId = userPlan.UserId,
            PlanId = userPlan.PlanId,
        };
    }

    public static Entities.UserPlan ToEntity(UserPlanResponse userPlanResponse)
    {
        return new Entities.UserPlan
        {
            Id = userPlanResponse.Id,
            UserId = userPlanResponse.UserId,
            PlanId = userPlanResponse.PlanId,
        };
    }
}
