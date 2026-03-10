using static Application.Common.ActivityHelper;
using Domain.UserPlan.Dto;
using Domain.UserPlan.Ports.In;
using Domain.UserPlan.Ports.Out;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.UserPlan;

public class UserPlanManager : IUserPlanManager
{
    private static readonly ActivitySource ActivitySource = new("users-api");

    private readonly ILogger<UserPlanManager> _logger;
    private readonly IUserPlanRepository _userPlanRepository;

    public UserPlanManager(ILogger<UserPlanManager> logger, IUserPlanRepository userPlanRepository)
    {
        _logger = logger;
        _userPlanRepository = userPlanRepository;
    }

    public async Task CreateAsync(UserPlanCreateRequest request)
    {
        using var activity = ActivitySource.StartActivity("UserPlanManager.CreateUserPlan", ActivityKind.Internal);
        activity?.SetTag("user.id", request.UserId);
        activity?.SetTag("plan.id", request.PlanId.ToString());
        _logger.LogInformation("Assigning plan {PlanId} to user {UserId}", request.PlanId, request.UserId);

        try
        {
            var entity = UserPlanCreateRequest.ToEntity(request);
            await _userPlanRepository.CreateAsync(entity);
            activity?.SetStatus(ActivityStatusCode.Ok);
            _logger.LogInformation("Plan {PlanId} assigned to user {UserId}", request.PlanId, request.UserId);
        }
        catch (Exception ex)
        {
            RecordExceptionSpan(activity, ex);
            _logger.LogError(ex, "Error assigning plan {PlanId} to user {UserId}", request.PlanId, request.UserId);
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        using var activity = ActivitySource.StartActivity("UserPlanManager.DeleteUserPlan", ActivityKind.Internal);
        activity?.SetTag("userplan.id", id.ToString());
        _logger.LogInformation("Deleting user plan {UserPlanId}", id);

        try
        {
            var entity = await _userPlanRepository.GetByIdAsync(id);
            _userPlanRepository.Delete(entity);
            activity?.SetStatus(ActivityStatusCode.Ok);
            _logger.LogInformation("User plan {UserPlanId} deleted", id);
        }
        catch (Exception ex)
        {
            RecordExceptionSpan(activity, ex);
            _logger.LogError(ex, "Error deleting user plan {UserPlanId}", id);
            throw;
        }
    }

    public async Task<List<UserPlanResponse>> GetAllAsync()
    {
        using var activity = ActivitySource.StartActivity("UserPlanManager.GetAll", ActivityKind.Internal);
        _logger.LogInformation("Fetching all user plans");

        try
        {
            var usersPlans = await _userPlanRepository.GetAllAsync();
            var dtos = usersPlans.Select(UserPlanResponse.ToDto).ToList();
            activity?.SetTag("userplan.count", dtos.Count.ToString());
            activity?.SetStatus(ActivityStatusCode.Ok);
            _logger.LogInformation("Fetched {Count} user plans", dtos.Count);
            return dtos;
        }
        catch (Exception ex)
        {
            RecordExceptionSpan(activity, ex);
            _logger.LogError(ex, "Error fetching all user plans");
            throw;
        }
    }

    public async Task<UserPlanResponse> GetByIdAsync(int id)
    {
        using var activity = ActivitySource.StartActivity("UserPlanManager.GetById", ActivityKind.Internal);
        activity?.SetTag("userplan.id", id.ToString());
        _logger.LogInformation("Fetching user plan {UserPlanId}", id);

        try
        {
            var result = await _userPlanRepository.GetByIdAsync(id)
                is var userPlanModel && userPlanModel is not null
                ? UserPlanResponse.ToDto(userPlanModel)
                : null!;
            activity?.SetStatus(ActivityStatusCode.Ok);
            return result;
        }
        catch (Exception ex)
        {
            RecordExceptionSpan(activity, ex);
            _logger.LogError(ex, "Error fetching user plan {UserPlanId}", id);
            throw;
        }
    }

    public async Task<UserPlanResponse> GetByUserId(string userId)
    {
        using var activity = ActivitySource.StartActivity("UserPlanManager.GetByUserId", ActivityKind.Internal);
        activity?.SetTag("user.id", userId);
        _logger.LogInformation("Fetching user plan for user {UserId}", userId);

        try
        {
            var result = await _userPlanRepository.GetByUserId(userId)
                is var userPlansModel && userPlansModel is not null
                ? UserPlanResponse.ToDto(userPlansModel)
                : null!;
            activity?.SetStatus(ActivityStatusCode.Ok);
            return result;
        }
        catch (Exception ex)
        {
            RecordExceptionSpan(activity, ex);
            _logger.LogError(ex, "Error fetching user plan for user {UserId}", userId);
            throw;
        }
    }

    public async Task UpdateAsync(UserPlanUpdateRequest request)
    {
        using var activity = ActivitySource.StartActivity("UserPlanManager.UpdateUserPlan", ActivityKind.Internal);
        activity?.SetTag("userplan.id", request.Id.ToString());
        _logger.LogInformation("Updating user plan {UserPlanId}", request.Id);

        try
        {
            var entity = await _userPlanRepository.GetByIdAsync(request.Id);

            if (entity is null)
            {
                _logger.LogWarning("User plan {UserPlanId} not found for update", request.Id);
                activity?.SetStatus(ActivityStatusCode.Error, "UserPlan not found");
                return;
            }

            entity.PlanId = request.PlanId;
            entity.UserId = request.UserId;

            await _userPlanRepository.UpdateAsync(entity);
            activity?.SetStatus(ActivityStatusCode.Ok);
            _logger.LogInformation("User plan {UserPlanId} updated", request.Id);
        }
        catch (Exception ex)
        {
            RecordExceptionSpan(activity, ex);
            _logger.LogError(ex, "Error updating user plan {UserPlanId}", request.Id);
            throw;
        }
    }


}
