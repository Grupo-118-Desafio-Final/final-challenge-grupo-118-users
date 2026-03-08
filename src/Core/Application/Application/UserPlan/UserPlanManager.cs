using Domain.UserPlan.Dto;
using Domain.UserPlan.Ports.In;
using Domain.UserPlan.Ports.Out;

namespace Application.UserPlan;

public class UserPlanManager : IUserPlanManager
{
    private readonly IUserPlanRepository _userPlanRepository;

    public UserPlanManager(IUserPlanRepository userPlanRepository)
    {
        _userPlanRepository = userPlanRepository;
    }

    public async Task CreateAsync(UserPlanCreateRequest request)
    {
        var entity = UserPlanCreateRequest.ToEntity(request);

        await _userPlanRepository.CreateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _userPlanRepository.GetByIdAsync(id);

        _userPlanRepository.Delete(entity);
    }

    public async Task<List<UserPlanResponse>> GetAllAsync()
    {
        var usersPlans = await _userPlanRepository.GetAllAsync();
        var dtos = usersPlans.Select(UserPlanResponse.ToDto).ToList();

        return dtos;
    }

    public async Task<UserPlanResponse> GetByIdAsync(int id)
    {
        return await _userPlanRepository.GetByIdAsync(id)
            is var userPlanModel && userPlanModel is not null
            ? UserPlanResponse.ToDto(userPlanModel)
            : null!;
    }

    public async Task<UserPlanResponse> GetByUserId(string userId)
    {
        return await _userPlanRepository.GetByUserId(userId)
            is var userPlansModel && userPlansModel is not null
            ? UserPlanResponse.ToDto(userPlansModel)
            : null!;
    }

    public async Task UpdateAsync(UserPlanUpdateRequest request)
    {
        var entity = await _userPlanRepository.GetByIdAsync(request.Id);

        if (entity is not null)
        {
            entity.PlanId = request.PlanId;
            entity.UserId = request.UserId;
        }

        await _userPlanRepository.UpdateAsync(entity);
    }
}
