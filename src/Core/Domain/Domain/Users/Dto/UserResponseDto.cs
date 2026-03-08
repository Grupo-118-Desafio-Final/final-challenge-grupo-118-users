using Domain.Users.Entities;

namespace Domain.Users.Dto;

public class UserResponseDto
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    public bool Error { get; set; }
    public string ErrorMessage { get; set; }


    public static UserResponseDto ToDto(User user)
    {
        return new UserResponseDto
        {
            Name = user.Name,
            LastName = user.LastName,
            Email = user.Email,
            BirthDate = user.BirthDate
        };
    }

    public static User ToEntity(UserResponseDto response)
    {
        return new User(
            response.Name,
            response.LastName,
            response.Email,
            response.BirthDate
        );
    }
}
