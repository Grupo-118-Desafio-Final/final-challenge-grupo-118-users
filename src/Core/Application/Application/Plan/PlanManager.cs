using Domain.Plan.Dto;
using Domain.Plan.Ports.In;
using Domain.Plan.Ports.Out;
using Domain.UserPlan.Ports.In;

namespace Application.Plans;

public class PlanManager : IPlanManager
{
    private readonly IPlanRepository _planRepository;
    private readonly IUserPlanManager _userPlanManager;
    public PlanManager(IPlanRepository planRepository, IUserPlanManager userPlanManager)
    {
        _planRepository = planRepository;
        _userPlanManager = userPlanManager;
    }

    public async Task CreateAsync(PlanCreateRequestDto planCreateRequestDto)
    {
        var entity = PlanCreateRequestDto.ToEntity(planCreateRequestDto);

        await _planRepository.CreateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _planRepository.GetByIdAsync(id);

        _planRepository.Delete(entity);
    }

    public async Task<List<PlanResponseDto>> GetAll()
    {
        var planList = await _planRepository.GetAll();
        var dtos = planList.Select(PlanResponseDto.ToDto).ToList();
        return dtos;
    }

    public async Task<PlanResponseDto> GetById(int id)
    {
        return await _planRepository.GetByIdAsync(id)
            is var planModel && planModel is not null
            ? PlanResponseDto.ToDto(planModel)
            : null!;
    }

    public async Task<PlanResponseDto> GetByNameAsync(string name)
    {
        return await _planRepository.GetByNameAsync(name)
            is var planModel && planModel is not null
            ? PlanResponseDto.ToDto(planModel)
            :null!;
    }

    public async Task<PlanResponseDto> GetPlanByUserId(string userId)
    {
        var userPlanResponse = await _userPlanManager.GetByUserId(userId);

        return await GetById(userPlanResponse.PlanId);
    }

    public async Task UpdateAsync(PlanUpdateRequestDto planUpdateRequestDto)
    {
        var existingPlan = await _planRepository.GetByIdAsync(planUpdateRequestDto.Id);

        if (existingPlan != null)
        {
            existingPlan.Name = planUpdateRequestDto.Name;
            existingPlan.Price = planUpdateRequestDto.Price;
        }

        await _planRepository.UpdateAsync(existingPlan);
    }
}
