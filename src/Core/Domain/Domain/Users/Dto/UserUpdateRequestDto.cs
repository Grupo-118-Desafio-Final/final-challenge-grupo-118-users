using Domain.Users.Entities;

namespace Domain.Users.Dto;

public class UserUpdateRequestDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }

    public static UserUpdateRequestDto ToDto(User user)
    {
        return new UserUpdateRequestDto
        {
            Name = user.Name,
            LastName = user.LastName,
            Email = user.Email,
            BirthDate = user.BirthDate
        };
    }

    public static User ToEntity(UserUpdateRequestDto request, Guid userId)
    {
        return new User(
            request.Name,
            request.LastName,
            request.Email,
            request.BirthDate
        );
    }
}
