using Domain.UserPlan.Dto;

namespace Domain.UserPlan.Ports.In;

public interface IUserPlanManager
{
    Task CreateAsync(UserPlanCreateRequest request);
    Task UpdateAsync(UserPlanUpdateRequest request);
    Task DeleteAsync(int id);
    Task<UserPlanResponse> GetByIdAsync(int id);
    Task<List<UserPlanResponse>> GetAllAsync();
    Task<UserPlanResponse> GetByUserId(string userId);
}
