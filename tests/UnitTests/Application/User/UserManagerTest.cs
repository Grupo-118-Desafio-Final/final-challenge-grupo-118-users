using NSubstitute;
using UserManager = global::Application.User.UserManager;
using UserEntity = global::Domain.Users.Entities.User;
using UserCreateRequestDto = global::Domain.Users.Dto.UserCreateRequestDto;
using UserUpdateRequestDto = global::Domain.Users.Dto.UserUpdateRequestDto;
using IPasswordManager = global::Domain.Users.Ports.In.IPasswordManager;
using IUserRepository = global::Domain.Users.Ports.Out.IUserRepository;

namespace UnitTests.Application.User;

public class UserManagerTest
{
    private readonly IPasswordManager _passwordManager;
    private readonly IUserRepository _userRepository;
    private readonly UserManager _sut;
    private static readonly DateTime DefaultBirthDate = new(1990, 5, 20);

    public UserManagerTest()
    {
        _passwordManager = Substitute.For<IPasswordManager>();
        _userRepository = Substitute.For<IUserRepository>();
        _sut = new UserManager(_passwordManager, _userRepository);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldHashPasswordAndPersistUser()
    {
        // Arrange
        var request = new UserCreateRequestDto
        {
            Name = "João",
            LastName = "Silva",
            Email = "joao@email.com",
            BirthDate = DefaultBirthDate,
            Password = "senha123"
        };
        _passwordManager
            .When(p => p.CreatePasswordHash(Arg.Any<string>(), out Arg.Any<string>()))
            .Do(call => call[1] = "hashed_senha123");

        // Act
        var result = await _sut.CreateUserAsync(request);

        // Assert
        _passwordManager.Received(1).CreatePasswordHash(request.Password, out Arg.Any<string>());
        await _userRepository.Received(1).CreateAsync(Arg.Is<UserEntity>(u =>
            u.Name == request.Name &&
            u.Email == request.Email
        ));
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Email, result.Email);
    }

    [Fact]
    public async Task DeleteUser_WhenUserExists_ShouldDeleteAndReturnDto()
    {
        // Arrange
        var user = new UserEntity("Ana", "Costa", "ana@email.com", DefaultBirthDate);
        _userRepository.GetByIdAsync(user.Id).Returns(user);

        // Act
        var result = await _sut.DeleteUser(user.Id);

        // Assert
        await _userRepository.Received(1).Delete(user);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task DeleteUser_WhenUserNotFound_ShouldNotCallDelete()
    {
        // Arrange
        var id = Guid.NewGuid();
        _userRepository.GetByIdAsync(id).Returns((UserEntity)null!);

        // Act
        // UserManager.DeleteUser calls UserResponseDto.ToDto(null) when user is not found,
        // causing NullReferenceException — Delete must not be called before that point.
        await Assert.ThrowsAsync<NullReferenceException>(() => _sut.DeleteUser(id));

        // Assert
        await _userRepository.DidNotReceive().Delete(Arg.Any<UserEntity>());
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnMappedDtoList()
    {
        // Arrange
        var users = new List<UserEntity>
        {
            new("João", "Silva", "joao@email.com", DefaultBirthDate),
            new("Maria", "Souza", "maria@email.com", DefaultBirthDate)
        };
        _userRepository.GetAll().Returns(users);

        // Act
        var result = await _sut.GetAllUsersAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("João", result[0].Name);
        Assert.Equal("Maria", result[1].Name);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnDto()
    {
        // Arrange
        var user = new UserEntity("Pedro", "Lima", "pedro@email.com", DefaultBirthDate);
        _userRepository.GetByEmailAsync("pedro@email.com").Returns(user);

        // Act
        var result = await _sut.GetUserByEmailAsync("pedro@email.com");

        // Assert
        Assert.Equal("Pedro", result.Name);
        Assert.Equal("pedro@email.com", result.Email);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnDto()
    {
        // Arrange
        var user = new UserEntity("Carla", "Mendes", "carla@email.com", DefaultBirthDate);
        _userRepository.GetByIdAsync(user.Id).Returns(user);

        // Act
        var result = await _sut.GetUserByIdAsync(user.Id);

        // Assert
        Assert.Equal("Carla", result.Name);
        Assert.Equal("carla@email.com", result.Email);
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ShouldReturnNotFoundMessage()
    {
        // Arrange
        _userRepository.GetByEmailAsync("naoexiste@email.com").Returns((UserEntity)null!);

        // Act
        var result = await _sut.LoginAsync("naoexiste@email.com", "senha");

        // Assert
        Assert.Equal("User not found.", result);
    }

    [Fact]
    public async Task LoginAsync_WhenUserExists_ShouldReturnJwtToken()
    {
        // Arrange
        var user = new UserEntity("João", "Silva", "joao@email.com", DefaultBirthDate);
        user.SetPassword("hashed");
        _userRepository.GetByEmailAsync("joao@email.com").Returns(user);
        _passwordManager.VerifyPassword("senha123", "hashed").Returns(true);
        _passwordManager.GenerateJwtToken(user).Returns("jwt_token_abc");

        // Act
        var result = await _sut.LoginAsync("joao@email.com", "senha123");

        // Assert
        Assert.Equal("jwt_token_abc", result);
    }

    [Fact]
    public async Task UpdateUserAsync_WhenUserExists_ShouldUpdateAndReturnDto()
    {
        // Arrange
        var existing = new UserEntity("Antigo", "Nome", "antigo@email.com", DefaultBirthDate);
        var request = new UserUpdateRequestDto
        {
            Id = existing.Id,
            Name = "Novo",
            LastName = "Nome",
            Email = "novo@email.com",
            BirthDate = DefaultBirthDate
        };
        _userRepository.GetByIdAsync(existing.Id).Returns(existing);

        // Act
        var result = await _sut.UpdateUserAsync(request);

        // Assert
        await _userRepository.Received(1).UpdateAsync(Arg.Any<UserEntity>());
        Assert.Equal("Novo", result.Name);
        Assert.Equal("novo@email.com", result.Email);
    }

    [Fact]
    public async Task UpdateUserAsync_WhenUserNotFound_ShouldReturnErrorDto()
    {
        // Arrange
        var request = new UserUpdateRequestDto { Id = Guid.NewGuid(), Name = "Qualquer" };
        _userRepository.GetByIdAsync(request.Id).Returns((UserEntity)null!);

        // Act
        var result = await _sut.UpdateUserAsync(request);

        // Assert
        Assert.True(result.Error);
        Assert.Equal("User not found.", result.ErrorMessage);
        await _userRepository.DidNotReceive().UpdateAsync(Arg.Any<UserEntity>());
    }
}
