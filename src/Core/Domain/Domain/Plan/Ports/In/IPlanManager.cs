using Domain.Plan.Dto;

namespace Domain.Plan.Ports.In;

public interface IPlanManager
{
    Task CreateAsync(PlanCreateRequestDto planCreateRequestDto);
    Task UpdateAsync(PlanUpdateRequestDto planUpdateRequestDto);
    Task DeleteAsync(int id);
    Task<PlanResponseDto> GetById(int id);
    Task<List<PlanResponseDto>> GetAll();
    Task<PlanResponseDto> GetByNameAsync(string name);
    Task<PlanResponseDto> GetPlanByUserId(string userId);
}
