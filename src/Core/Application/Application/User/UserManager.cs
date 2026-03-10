using Domain.Plan.Ports.In;
using Domain.UserPlan.Dto;
using Domain.UserPlan.Ports.In;
using Domain.Users.Dto;
using Domain.Users.Ports.In;
using Domain.Users.Ports.Out;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Application.User;

public class UserManager : IUserManager
{
    private static readonly ActivitySource ActivitySource = new("users-api");

    private readonly ILogger<UserManager> _logger;
    private readonly IPasswordManager _passwordManager;
    private readonly IUserRepository _userRepository;
    private readonly IUserPlanManager _userPlanManager;
    private readonly IPlanManager _planManager;

    public UserManager(
        ILogger<UserManager> logger,
        IPasswordManager passwordManager,
        IUserRepository userRepository,
        IUserPlanManager userPlanManager,
        IPlanManager planManager)
    {
        _logger = logger;
        _passwordManager = passwordManager;
        _userRepository = userRepository;
        _userPlanManager = userPlanManager;
        _planManager = planManager;
    }

    public async Task<UserResponseDto> CreateUserAsync(UserCreateRequestDto request)
    {
        using var activity = ActivitySource.StartActivity("CreateUser", ActivityKind.Internal);
        activity?.SetTag("user.email", request.Email);
        _logger.LogInformation("Creating user with email {Email}", request.Email);

        try
        {
            var user = UserCreateRequestDto.ToEntity(request);
            _passwordManager.CreatePasswordHash(request.Password, out var passwordHash);
            user.SetPassword(passwordHash);

            activity?.AddEvent(new ActivityEvent("persisting user"));
            await _userRepository.CreateAsync(user);
            activity?.SetTag("user.id", user.Id.ToString());
            _logger.LogInformation("User {UserId} persisted", user.Id);

            activity?.AddEvent(new ActivityEvent("fetching default plan"));
            var planDto = await _planManager.GetByNameAsync("Default");

            var userPlanRequest = new UserPlanCreateRequest
            {
                UserId = user.Id.ToString(),
                PlanId = planDto.Id
            };

            activity?.AddEvent(new ActivityEvent("assigning plan to user"));
            await _userPlanManager.CreateAsync(userPlanRequest);
            _logger.LogInformation("Plan {PlanId} assigned to user {UserId}", planDto.Id, user.Id);

            activity?.SetStatus(ActivityStatusCode.Ok);
            return UserResponseDto.ToDto(user);
        }
        catch (Exception ex)
        {
            RecordExceptionSpan(activity, ex);
            _logger.LogError(ex, "Error creating user with email {Email}", request.Email);
            throw;
        }
    }

    public async Task<UserResponseDto> DeleteUser(Guid id)
    {
        using var activity = ActivitySource.StartActivity("UserManager.DeleteUser", ActivityKind.Internal);
        activity?.SetTag("user.id", id.ToString());
        _logger.LogInformation("Deleting user {UserId}", id);

        try
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
            {
                _logger.LogWarning("User {UserId} not found for deletion", id);
                activity?.SetStatus(ActivityStatusCode.Error, "User not found");
                return new UserResponseDto { Error = true, ErrorMessage = "User not found." };
            }

            await _userRepository.Delete(user);
            _logger.LogInformation("User {UserId} deleted", id);
            activity?.SetStatus(ActivityStatusCode.Ok);
            return UserResponseDto.ToDto(user);
        }
        catch (Exception ex)
        {
            RecordExceptionSpan(activity, ex);
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            throw;
        }
    }

    public async Task<List<UserResponseDto>> GetAllUsersAsync()
    {
        using var activity = ActivitySource.StartActivity("UserManager.GetAllUsers", ActivityKind.Internal);
        _logger.LogInformation("Fetching all users");

        try
        {
            var userList = await _userRepository.GetAll();
            var result = userList.Select(UserResponseDto.ToDto).ToList();
            activity?.SetTag("user.count", result.Count.ToString());
            activity?.SetStatus(ActivityStatusCode.Ok);
            _logger.LogInformation("Fetched {Count} users", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            RecordExceptionSpan(activity, ex);
            _logger.LogError(ex, "Error fetching all users");
            throw;
        }
    }

    public async Task<UserResponseDto> GetUserByEmailAsync(string email)
    {
        using var activity = ActivitySource.StartActivity("UserManager.GetUserByEmail", ActivityKind.Internal);
        activity?.SetTag("user.email", email);
        _logger.LogInformation("Fetching user by email {Email}", email);

        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            activity?.SetStatus(ActivityStatusCode.Ok);
            return UserResponseDto.ToDto(user);
        }
        catch (Exception ex)
        {
            RecordExceptionSpan(activity, ex);
            _logger.LogError(ex, "Error fetching user by email {Email}", email);
            throw;
        }
    }

    public async Task<UserResponseDto> GetUserByIdAsync(Guid id)
    {
        using var activity = ActivitySource.StartActivity("UserManager.GetUserById", ActivityKind.Internal);
        activity?.SetTag("user.id", id.ToString());
        _logger.LogInformation("Fetching user {UserId}", id);

        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            activity?.SetStatus(ActivityStatusCode.Ok);
            return UserResponseDto.ToDto(user);
        }
        catch (Exception ex)
        {
            RecordExceptionSpan(activity, ex);
            _logger.LogError(ex, "Error fetching user {UserId}", id);
            throw;
        }
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        using var activity = ActivitySource.StartActivity("UserManager.Login", ActivityKind.Internal);
        activity?.SetTag("user.email", email);
        _logger.LogInformation("Login attempt for email {Email}", email);

        try
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user is null)
            {
                _logger.LogWarning("Login failed: user {Email} not found", email);
                activity?.SetStatus(ActivityStatusCode.Error, "User not found");
                return "User not found.";
            }

            if (!_passwordManager.VerifyPassword(password, user.Password))
            {
                _logger.LogWarning("Login failed: invalid password for {Email}", email);
                activity?.SetStatus(ActivityStatusCode.Error, "Invalid password");
                return "Invalid password.";
            }

            var token = _passwordManager.GenerateJwtToken(user);
            _logger.LogInformation("Login successful for user {UserId}", user.Id);
            activity?.SetTag("user.id", user.Id.ToString());
            activity?.SetStatus(ActivityStatusCode.Ok);
            return token;
        }
        catch (Exception ex)
        {
            RecordExceptionSpan(activity, ex);
            _logger.LogError(ex, "Error during login for email {Email}", email);
            throw;
        }
    }

    public async Task<UserResponseDto> UpdateUserAsync(UserUpdateRequestDto request)
    {
        using var activity = ActivitySource.StartActivity("UserManager.UpdateUser", ActivityKind.Internal);
        activity?.SetTag("user.id", request.Id.ToString());
        _logger.LogInformation("Updating user {UserId}", request.Id);

        try
        {
            var user = await _userRepository.GetByIdAsync(request.Id);

            if (user is null)
            {
                _logger.LogWarning("User {UserId} not found for update", request.Id);
                activity?.SetStatus(ActivityStatusCode.Error, "User not found");
                return new UserResponseDto { ErrorMessage = "User not found.", Error = true };
            }

            user = UserUpdateRequestDto.ToEntity(request, user.Id);
            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("User {UserId} updated", request.Id);
            activity?.SetStatus(ActivityStatusCode.Ok);
            return UserResponseDto.ToDto(user);
        }
        catch (Exception ex)
        {
            RecordExceptionSpan(activity, ex);
            _logger.LogError(ex, "Error updating user {UserId}", request.Id);
            throw;
        }
    }

    /// <summary>
    /// Registra a exceção no span do OpenTelemetry. Excluído da cobertura pois é
    /// infraestrutura de observabilidade — coberto pelos testes de integração.
    /// </summary>
    [ExcludeFromCodeCoverage]
    private static void RecordExceptionSpan(Activity? activity, Exception ex)
    {
        activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
        activity?.AddEvent(new ActivityEvent("exception", tags: new ActivityTagsCollection
        {
            { "exception.type", ex.GetType().FullName },
            { "exception.message", ex.Message },
            { "exception.stacktrace", ex.StackTrace }
        }));
    }
}
