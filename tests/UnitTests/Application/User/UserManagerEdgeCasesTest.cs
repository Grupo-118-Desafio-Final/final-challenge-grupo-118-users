using Microsoft.Extensions.Logging;
using NSubstitute;
using UnitTests.Helpers;
using UserManager = Application.User.UserManager;
using UserEntity = Domain.Users.Entities.User;
using PlanEntity = Domain.Plan.Entities.Plan;
using IPasswordManager = Domain.Users.Ports.In.IPasswordManager;
using IUserRepository = Domain.Users.Ports.Out.IUserRepository;
using IUserPlanManager = Domain.UserPlan.Ports.In.IUserPlanManager;
using IPlanManager = Domain.Plan.Ports.In.IPlanManager;
using PlanResponseDto = Domain.Plan.Dto.PlanResponseDto;

namespace UnitTests.Application.User;

public class UserManagerEdgeCasesTest
{
    private readonly ILogger<UserManager> _logger;
    private readonly IPasswordManager _passwordManager;
    private readonly IUserRepository _userRepository;
    private readonly IUserPlanManager _userPlanManager;
    private readonly IPlanManager _planManager;
    private readonly UserManager _sut;
    private static readonly DateTime DefaultBirthDate = new(1990, 5, 20);

    public UserManagerEdgeCasesTest()
    {
        _logger = Substitute.For<ILogger<UserManager>>();
        _passwordManager = Substitute.For<IPasswordManager>();
        _userRepository = Substitute.For<IUserRepository>();
        _userPlanManager = Substitute.For<IUserPlanManager>();
        _planManager = Substitute.For<IPlanManager>();
        _sut = new UserManager(
            _logger,
            _passwordManager,
            _userRepository,
            _userPlanManager,
            _planManager);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIncorrect_ShouldReturnInvalidPasswordMessage()
    {
        // Arrange
        var user = new UserEntity("João", "Silva", "joao@email.com", DefaultBirthDate);
        user.SetPassword("hashed_password");
        _userRepository.GetByEmailAsync("joao@email.com").Returns(user);
        _passwordManager.VerifyPassword("wrong_password", "hashed_password").Returns(false);

        // Act
        var result = await _sut.LoginAsync("joao@email.com", "wrong_password");

        // Assert
        _passwordManager.Received(1).VerifyPassword("wrong_password", "hashed_password");
        Assert.Equal("Invalid password.", result);
        _passwordManager.DidNotReceive().GenerateJwtToken(Arg.Any<UserEntity>(), Arg.Any<PlanEntity>());
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

        // Act & Assert — ToDto é chamado com null, o que lança NullReferenceException
        await Assert.ThrowsAsync<NullReferenceException>(
            () => _sut.GetUserByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetUserByEmailAsync_WhenUserNotFound_ShouldThrowNullReferenceException()
    {
        // Arrange
        _userRepository.GetByEmailAsync("naoexiste@email.com").Returns((UserEntity)null!);

        // Act & Assert — ToDto é chamado com null, o que lança NullReferenceException
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
        _planManager.GetById(1).Returns(new PlanResponseDto { Id = 1, Name = "Default" });

        var request = new global::Domain.Users.Dto.UserCreateRequestDto
        {
            Name = "Test",
            LastName = "User",
            Email = "test@email.com",
            BirthDate = DefaultBirthDate,
            Password = "password",
            PlanId = 1
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

    [Fact]
    public async Task CreateUserAsync_WhenRepositoryThrows_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var request = new global::Domain.Users.Dto.UserCreateRequestDto
        {
            Name = "Test",
            LastName = "User",
            Email = "test@email.com",
            BirthDate = DefaultBirthDate,
            Password = "password"
        };
        _planManager.GetByNameAsync("Default").Returns(new PlanResponseDto { Id = 1, Name = "Default" });
        var expectedException = new InvalidOperationException("Erro no banco de dados");
        _userRepository
            .When(r => r.CreateAsync(Arg.Any<UserEntity>()))
            .Do(_ => throw expectedException);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateUserAsync(request));
        _logger.ShouldHaveLoggedError(expectedException);
    }

    [Fact]
    public async Task GetAllUsersAsync_WhenRepositoryThrows_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Erro de conexão");
        _userRepository
            .When(r => r.GetAll())
            .Do(_ => throw expectedException);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(_sut.GetAllUsersAsync);
        _logger.ShouldHaveLoggedError(expectedException);
    }
}
