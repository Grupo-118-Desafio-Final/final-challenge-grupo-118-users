using Domain.Users.Dto;
using Domain.Users.Ports.In;
using Domain.Users.Ports.Out;

namespace Application.User;

public class UserManager : IUserManager
{
    private readonly IPasswordManager _passwordManager;
    private readonly IUserRepository _userRepository;

    public UserManager(IPasswordManager passwordManager, IUserRepository userRepository)
    {
        _passwordManager = passwordManager;
        _userRepository = userRepository;
    }

    public async Task<UserResponseDto> CreateUserAsync(UserCreateRequestDto request)
    {
        try
        {
            var user = UserCreateRequestDto.ToEntity(request);
            _passwordManager.CreatePasswordHash(request.Password, out var passwordHash);
            user.SetPassword(passwordHash);
            await _userRepository.CreateAsync(user);

            return UserResponseDto.ToDto(user);
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public async Task<UserResponseDto> DeleteUser(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user != null)
            _userRepository.Delete(user);

        return UserResponseDto.ToDto(user);
    }

    public async Task<List<UserResponseDto>> GetAllUsersAsync()
    {
        var userlist =  await _userRepository.GetAll();

        var result = new List<UserResponseDto>();

        foreach (var user in userlist)
        {
            result.Add(UserResponseDto.ToDto(user));
        }

        return result;
    }

    public async Task<UserResponseDto> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return UserResponseDto.ToDto(user);
    }

    public async Task<UserResponseDto> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return UserResponseDto.ToDto(user);
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return "User not found.";

        if (!_passwordManager.VerifyPassword(password, user.Password))
        {

        }

        return _passwordManager.GenerateJwtToken(user);
    }

    public async Task<UserResponseDto> UpdateUserAsync(UserUpdateRequestDto request)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);

        if(user == null)
        {
            return new UserResponseDto
            {
                ErrorMessage = "User not found.",
                Error = true
            };
        }

        user = UserUpdateRequestDto.ToEntity(request, user.Id);
        await _userRepository.UpdateAsync(user);

        return UserResponseDto.ToDto(user);
    }
}
