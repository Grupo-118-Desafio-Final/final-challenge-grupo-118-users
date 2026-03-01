using Domain.Base.Entities;
using Domain.Plan.ValueObjects;
using UserPlanEntity = Domain.UserPlan.Entities.UserPlan;

namespace Domain.Plan.Entities;

public class Plan : BaseEntity
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public ImageQualityEnum ImageQuality { get; set; } = ImageQualityEnum.Hd;
    public string MaxSizeInMegaBytes { get; set; } = "200";
    public string MaxDurationInSeconds { get; set; } = "20";
    public string Threads { get; set; } = "1";
    public ICollection<UserPlanEntity> UsersPlans { get; set; } = new List<UserPlanEntity>();
}
