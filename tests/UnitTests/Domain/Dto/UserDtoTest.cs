using Domain.Users.Dto;
using Domain.Users.Entities;

namespace UnitTests.Domain.Dto;

public class UserDtoTest
{
    private static readonly DateTime DefaultBirthDate = new(1990, 5, 20);

    // --- UserCreateRequestDto ---

    [Fact]
    public void UserCreateRequestDto_ToDto_ShouldMapAllFieldsFromUser()
    {
        var user = new User("João", "Silva", "joao@email.com", DefaultBirthDate);

        var dto = UserCreateRequestDto.ToDto(user);

        Assert.Equal("João", dto.Name);
        Assert.Equal("Silva", dto.LastName);
        Assert.Equal("joao@email.com", dto.Email);
        Assert.Equal(DefaultBirthDate, dto.BirthDate);
    }

    [Fact]
    public void UserCreateRequestDto_ToEntity_ShouldCreateUserWithCorrectFields()
    {
        var dto = new UserCreateRequestDto
        {
            Name = "Maria",
            LastName = "Costa",
            Email = "maria@email.com",
            BirthDate = DefaultBirthDate,
            Password = "senha123"
        };

        var user = UserCreateRequestDto.ToEntity(dto);

        Assert.Equal("Maria", user.Name);
        Assert.Equal("Costa", user.LastName);
        Assert.Equal("maria@email.com", user.Email);
        Assert.Equal(DefaultBirthDate, user.BirthDate);
        Assert.NotEqual(Guid.Empty, user.Id);
    }

    [Fact]
    public void UserCreateRequestDto_ToEntity_ShouldGenerateNewId()
    {
        var dto = new UserCreateRequestDto
        {
            Name = "Carlos",
            LastName = "Lima",
            Email = "carlos@email.com",
            BirthDate = DefaultBirthDate
        };

        var user1 = UserCreateRequestDto.ToEntity(dto);
        var user2 = UserCreateRequestDto.ToEntity(dto);

        Assert.NotEqual(user1.Id, user2.Id);
    }

    // --- UserResponseDto ---

    [Fact]
    public void UserResponseDto_ToDto_ShouldMapAllFieldsFromUser()
    {
        var user = new User("Ana", "Pereira", "ana@email.com", DefaultBirthDate);

        var dto = UserResponseDto.ToDto(user);

        Assert.Equal("Ana", dto.Name);
        Assert.Equal("Pereira", dto.LastName);
        Assert.Equal("ana@email.com", dto.Email);
        Assert.Equal(DefaultBirthDate, dto.BirthDate);
        Assert.False(dto.Error);
        Assert.Null(dto.ErrorMessage);
    }

    [Fact]
    public void UserResponseDto_ToEntity_ShouldCreateUserWithCorrectFields()
    {
        var responseDto = new UserResponseDto
        {
            Name = "Pedro",
            LastName = "Mendes",
            Email = "pedro@email.com",
            BirthDate = DefaultBirthDate
        };

        var user = UserResponseDto.ToEntity(responseDto);

        Assert.Equal("Pedro", user.Name);
        Assert.Equal("Mendes", user.LastName);
        Assert.Equal("pedro@email.com", user.Email);
        Assert.Equal(DefaultBirthDate, user.BirthDate);
    }

    [Fact]
    public void UserResponseDto_ErrorProperties_ShouldBeSettable()
    {
        var dto = new UserResponseDto
        {
            Error = true,
            ErrorMessage = "User not found."
        };

        Assert.True(dto.Error);
        Assert.Equal("User not found.", dto.ErrorMessage);
    }

    // --- UserUpdateRequestDto ---

    [Fact]
    public void UserUpdateRequestDto_ToDto_ShouldMapAllFieldsFromUser()
    {
        var user = new User("Lucas", "Ferreira", "lucas@email.com", DefaultBirthDate);

        var dto = UserUpdateRequestDto.ToDto(user);

        Assert.Equal("Lucas", dto.Name);
        Assert.Equal("Ferreira", dto.LastName);
        Assert.Equal("lucas@email.com", dto.Email);
        Assert.Equal(DefaultBirthDate, dto.BirthDate);
    }

    [Fact]
    public void UserUpdateRequestDto_ToEntity_ShouldCreateUserWithCorrectFields()
    {
        var dto = new UserUpdateRequestDto
        {
            Id = Guid.NewGuid(),
            Name = "Renata",
            LastName = "Souza",
            Email = "renata@email.com",
            BirthDate = DefaultBirthDate
        };

        var user = UserUpdateRequestDto.ToEntity(dto, dto.Id);

        Assert.Equal("Renata", user.Name);
        Assert.Equal("Souza", user.LastName);
        Assert.Equal("renata@email.com", user.Email);
        Assert.Equal(DefaultBirthDate, user.BirthDate);
    }

    [Fact]
    public void UserUpdateRequestDto_IdProperty_ShouldBeSettable()
    {
        var id = Guid.NewGuid();
        var dto = new UserUpdateRequestDto { Id = id };

        Assert.Equal(id, dto.Id);
    }
}
