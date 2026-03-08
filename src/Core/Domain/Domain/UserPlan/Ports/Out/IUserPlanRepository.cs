namespace Domain.UserPlan.Ports.Out;

public interface IUserPlanRepository
{
    Task CreateAsync(Entities.UserPlan userPlan);
    Task UpdateAsync(Entities.UserPlan userPlan);
    void Delete(Entities.UserPlan userPlan);
    Task<Entities.UserPlan> GetByIdAsync(int id);
    Task<List<Entities.UserPlan>> GetAllAsync();
    Task<Entities.UserPlan> GetByUserId(string userId);
}
