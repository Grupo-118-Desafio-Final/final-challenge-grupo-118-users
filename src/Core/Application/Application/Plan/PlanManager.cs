using Domain.Plan.Dto;
using Domain.Plan.Ports.In;
using Domain.Plan.Ports.Out;
using Domain.UserPlan.Ports.In;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Plans;

public class PlanManager : IPlanManager
{
    private static readonly ActivitySource ActivitySource = new("users-api");

    private readonly ILogger<PlanManager> _logger;
    private readonly IPlanRepository _planRepository;
    private readonly IUserPlanManager _userPlanManager;

    public PlanManager(ILogger<PlanManager> logger, IPlanRepository planRepository, IUserPlanManager userPlanManager)
    {
        _logger = logger;
        _planRepository = planRepository;
        _userPlanManager = userPlanManager;
    }

    public async Task CreateAsync(PlanCreateRequestDto planCreateRequestDto)
    {
        using var activity = ActivitySource.StartActivity("PlanManager.CreatePlan", ActivityKind.Internal);
        activity?.SetTag("plan.name", planCreateRequestDto.Name);
        _logger.LogInformation("Creating plan with name {PlanName}", planCreateRequestDto.Name);

        try
        {
            var entity = PlanCreateRequestDto.ToEntity(planCreateRequestDto);
            await _planRepository.CreateAsync(entity);
            activity?.SetTag("plan.id", entity.Id.ToString());
            activity?.SetStatus(ActivityStatusCode.Ok);
            _logger.LogInformation("Plan {PlanId} created", entity.Id);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddEvent(new ActivityEvent("exception", tags: new ActivityTagsCollection
            {
                { "exception.type", ex.GetType().FullName },
                { "exception.message", ex.Message },
                { "exception.stacktrace", ex.StackTrace }
            }));
            _logger.LogError(ex, "Error creating plan with name {PlanName}", planCreateRequestDto.Name);
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        using var activity = ActivitySource.StartActivity("PlanManager.DeletePlan", ActivityKind.Internal);
        activity?.SetTag("plan.id", id.ToString());
        _logger.LogInformation("Deleting plan {PlanId}", id);

        try
        {
            var entity = await _planRepository.GetByIdAsync(id);
            _planRepository.Delete(entity);
            activity?.SetStatus(ActivityStatusCode.Ok);
            _logger.LogInformation("Plan {PlanId} deleted", id);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddEvent(new ActivityEvent("exception", tags: new ActivityTagsCollection
            {
                { "exception.type", ex.GetType().FullName },
                { "exception.message", ex.Message },
                { "exception.stacktrace", ex.StackTrace }
            }));
            _logger.LogError(ex, "Error deleting plan {PlanId}", id);
            throw;
        }
    }

    public async Task<List<PlanResponseDto>> GetAll()
    {
        using var activity = ActivitySource.StartActivity("PlanManager.GetAll", ActivityKind.Internal);
        _logger.LogInformation("Fetching all plans");

        try
        {
            var planList = await _planRepository.GetAll();
            var dtos = planList.Select(PlanResponseDto.ToDto).ToList();
            activity?.SetTag("plan.count", dtos.Count.ToString());
            activity?.SetStatus(ActivityStatusCode.Ok);
            _logger.LogInformation("Fetched {Count} plans", dtos.Count);
            return dtos;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddEvent(new ActivityEvent("exception", tags: new ActivityTagsCollection
            {
                { "exception.type", ex.GetType().FullName },
                { "exception.message", ex.Message },
                { "exception.stacktrace", ex.StackTrace }
            }));
            _logger.LogError(ex, "Error fetching all plans");
            throw;
        }
    }

    public async Task<PlanResponseDto> GetById(int id)
    {
        using var activity = ActivitySource.StartActivity("PlanManager.GetById", ActivityKind.Internal);
        activity?.SetTag("plan.id", id.ToString());
        _logger.LogInformation("Fetching plan {PlanId}", id);

        try
        {
            var result = await _planRepository.GetByIdAsync(id)
                is var planModel && planModel is not null
                ? PlanResponseDto.ToDto(planModel)
                : null!;
            activity?.SetStatus(ActivityStatusCode.Ok);
            return result;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddEvent(new ActivityEvent("exception", tags: new ActivityTagsCollection
            {
                { "exception.type", ex.GetType().FullName },
                { "exception.message", ex.Message },
                { "exception.stacktrace", ex.StackTrace }
            }));
            _logger.LogError(ex, "Error fetching plan {PlanId}", id);
            throw;
        }
    }

    public async Task<PlanResponseDto> GetByNameAsync(string name)
    {
        using var activity = ActivitySource.StartActivity("PlanManager.GetByName", ActivityKind.Internal);
        activity?.SetTag("plan.name", name);
        _logger.LogInformation("Fetching plan by name {PlanName}", name);

        try
        {
            var result = await _planRepository.GetByNameAsync(name)
                is var planModel && planModel is not null
                ? PlanResponseDto.ToDto(planModel)
                : null!;
            activity?.SetStatus(ActivityStatusCode.Ok);
            return result;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddEvent(new ActivityEvent("exception", tags: new ActivityTagsCollection
            {
                { "exception.type", ex.GetType().FullName },
                { "exception.message", ex.Message },
                { "exception.stacktrace", ex.StackTrace }
            }));
            _logger.LogError(ex, "Error fetching plan by name {PlanName}", name);
            throw;
        }
    }

    public async Task<PlanResponseDto> GetPlanByUserId(string userId)
    {
        using var activity = ActivitySource.StartActivity("PlanManager.GetPlanByUserId", ActivityKind.Internal);
        activity?.SetTag("user.id", userId);
        _logger.LogInformation("Fetching plan for user {UserId}", userId);

        try
        {
            var userPlanResponse = await _userPlanManager.GetByUserId(userId);
            var result = await GetById(userPlanResponse.PlanId);
            activity?.SetStatus(ActivityStatusCode.Ok);
            return result;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddEvent(new ActivityEvent("exception", tags: new ActivityTagsCollection
            {
                { "exception.type", ex.GetType().FullName },
                { "exception.message", ex.Message },
                { "exception.stacktrace", ex.StackTrace }
            }));
            _logger.LogError(ex, "Error fetching plan for user {UserId}", userId);
            throw;
        }
    }

    public async Task UpdateAsync(PlanUpdateRequestDto planUpdateRequestDto)
    {
        using var activity = ActivitySource.StartActivity("PlanManager.UpdatePlan", ActivityKind.Internal);
        activity?.SetTag("plan.id", planUpdateRequestDto.Id.ToString());
        _logger.LogInformation("Updating plan {PlanId}", planUpdateRequestDto.Id);

        try
        {
            var existingPlan = await _planRepository.GetByIdAsync(planUpdateRequestDto.Id);

            if (existingPlan is null)
            {
                _logger.LogWarning("Plan {PlanId} not found for update", planUpdateRequestDto.Id);
                activity?.SetStatus(ActivityStatusCode.Error, "Plan not found");
                return;
            }

            existingPlan.Name = planUpdateRequestDto.Name;
            existingPlan.Price = planUpdateRequestDto.Price;

            await _planRepository.UpdateAsync(existingPlan);
            activity?.SetStatus(ActivityStatusCode.Ok);
            _logger.LogInformation("Plan {PlanId} updated", planUpdateRequestDto.Id);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddEvent(new ActivityEvent("exception", tags: new ActivityTagsCollection
            {
                { "exception.type", ex.GetType().FullName },
                { "exception.message", ex.Message },
                { "exception.stacktrace", ex.StackTrace }
            }));
            _logger.LogError(ex, "Error updating plan {PlanId}", planUpdateRequestDto.Id);
            throw;
        }
    }
}
