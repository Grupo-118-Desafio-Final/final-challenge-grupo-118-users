using Domain.Users.Dto;

namespace Domain.Users.Ports.In;

public interface IUserManager
{
    Task<UserResponseDto> CreateUserAsync(UserCreateRequestDto request);
    Task<UserResponseDto> UpdateUserAsync(UserUpdateRequestDto request);
    Task<UserResponseDto> DeleteUser(Guid id);
    Task<UserResponseDto> GetUserByIdAsync(Guid id);
    Task<List<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto> GetUserByEmailAsync(string email);
    Task<string> LoginAsync(string email, string password);
}
