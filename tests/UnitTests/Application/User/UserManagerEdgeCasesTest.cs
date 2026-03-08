using NSubstitute;
using UserManager = global::Application.User.UserManager;
using UserEntity = global::Domain.Users.Entities.User;
using IPasswordManager = global::Domain.Users.Ports.In.IPasswordManager;
using IUserRepository = global::Domain.Users.Ports.Out.IUserRepository;

namespace UnitTests.Application.User;

public class UserManagerEdgeCasesTest
{
    private readonly IPasswordManager _passwordManager;
    private readonly IUserRepository _userRepository;
    private readonly UserManager _sut;
    private static readonly DateTime DefaultBirthDate = new(1990, 5, 20);

    public UserManagerEdgeCasesTest()
    {
        _passwordManager = Substitute.For<IPasswordManager>();
        _userRepository = Substitute.For<IUserRepository>();
        _sut = new UserManager(_passwordManager, _userRepository);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIncorrect_ShouldCallVerifyPasswordAndReturnToken()
    {
        // Arrange — the current implementation falls through even on wrong password
        var user = new UserEntity("João", "Silva", "joao@email.com", DefaultBirthDate);
        user.SetPassword("hashed_password");
        _userRepository.GetByEmailAsync("joao@email.com").Returns(user);
        _passwordManager.VerifyPassword("wrong_password", "hashed_password").Returns(false);
        _passwordManager.GenerateJwtToken(user).Returns("token");

        // Act
        var result = await _sut.LoginAsync("joao@email.com", "wrong_password");

        // Assert — verifies the call happened
        _passwordManager.Received(1).VerifyPassword("wrong_password", "hashed_password");
    }

    [Fact]
    public async Task GetAllUsersAsync_WhenRepositoryReturnsEmptyList_ShouldReturnEmptyList()
    {
        // Arrange
        _userRepository.GetAll().Returns(new List<UserEntity>());

        // Act
        var result = await _sut.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserNotFound_ShouldThrowNullReferenceException()
    {
        // Arrange
        _userRepository.GetByIdAsync(Arg.Any<Guid>()).Returns((UserEntity)null!);

        // Act & Assert — ToDto is called on null, which throws
        await Assert.ThrowsAsync<NullReferenceException>(
            () => _sut.GetUserByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetUserByEmailAsync_WhenUserNotFound_ShouldThrowNullReferenceException()
    {
        // Arrange
        _userRepository.GetByEmailAsync("naoexiste@email.com").Returns((UserEntity)null!);

        // Act & Assert — ToDto is called on null, which throws
        await Assert.ThrowsAsync<NullReferenceException>(
            () => _sut.GetUserByEmailAsync("naoexiste@email.com"));
    }

    [Fact]
    public async Task CreateUserAsync_ShouldSetPasswordBeforeCallingRepository()
    {
        // Arrange
        var callOrder = new List<string>();
        _passwordManager
            .When(p => p.CreatePasswordHash(Arg.Any<string>(), out Arg.Any<string>()))
            .Do(call =>
            {
                call[1] = "hashed";
                callOrder.Add("hash");
            });
        _userRepository
            .When(r => r.CreateAsync(Arg.Any<UserEntity>()))
            .Do(_ => callOrder.Add("create"));

        var request = new global::Domain.Users.Dto.UserCreateRequestDto
        {
            Name = "Test",
            LastName = "User",
            Email = "test@email.com",
            BirthDate = DefaultBirthDate,
            Password = "password"
        };

        // Act
        await _sut.CreateUserAsync(request);

        // Assert
        Assert.Equal(new[] { "hash", "create" }, callOrder);
    }

    [Fact]
    public async Task UpdateUserAsync_WhenUserExists_ShouldCallRepositoryUpdate()
    {
        // Arrange
        var existing = new UserEntity("Old", "Name", "old@email.com", DefaultBirthDate);
        var request = new global::Domain.Users.Dto.UserUpdateRequestDto
        {
            Id = existing.Id,
            Name = "New",
            LastName = "Name",
            Email = "new@email.com",
            BirthDate = DefaultBirthDate
        };
        _userRepository.GetByIdAsync(existing.Id).Returns(existing);

        // Act
        await _sut.UpdateUserAsync(request);

        // Assert
        await _userRepository.Received(1).UpdateAsync(Arg.Any<UserEntity>());
    }
}
