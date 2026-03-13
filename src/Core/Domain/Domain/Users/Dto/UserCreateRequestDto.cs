using Domain.Users.Entities;

namespace Domain.Users.Dto;

public class UserCreateRequestDto
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    public string Password { get; set; }
    public int PlanId { get; set; }

    public static UserCreateRequestDto ToDto(User user)
    {
        return new UserCreateRequestDto
        {
            Name = user.Name,
            LastName = user.LastName,
            Email = user.Email,
            BirthDate = user.BirthDate
        };
    }

    public static User ToEntity(UserCreateRequestDto request)
    {
        return new User(
            request.Name,
            request.LastName,
            request.Email,
            request.BirthDate
        );
    }
}
