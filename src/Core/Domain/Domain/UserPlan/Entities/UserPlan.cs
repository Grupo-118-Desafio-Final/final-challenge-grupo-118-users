using Domain.Base.Entities;
using PlanEntity =  Domain.Plan.Entities.Plan;

namespace Domain.UserPlan.Entities;

public class UserPlan : BaseEntity
{
    public string UserId { get; set; } = null!;
    public int PlanId { get; set; }
    public PlanEntity Plan { get; set; } = null!;
}
