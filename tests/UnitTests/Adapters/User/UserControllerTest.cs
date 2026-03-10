using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using FinalChallengeUsers.API.Controllers;
using FinalChallengeUsers.API.Model;
using UserResponseDto = Domain.Users.Dto.UserResponseDto;
using UserCreateRequestDto = Domain.Users.Dto.UserCreateRequestDto;
using UserUpdateRequestDto = Domain.Users.Dto.UserUpdateRequestDto;
using IUserManager = Domain.Users.Ports.In.IUserManager;

namespace UnitTests.Adapters.User;

public class UserControllerTest
{
    private readonly IUserManager _userManager;
    private readonly UserController _sut;
    private static readonly DateTime DefaultBirthDate = new(1990, 5, 20);

    public UserControllerTest()
    {
        _userManager = Substitute.For<IUserManager>();
        _sut = new UserController(_userManager);
    }

    [Fact]
    public async Task Login_WhenCredentialsAreValid_ShouldReturnOkWithToken()
    {
        // Arrange
        var loginParams = new LoginParameters { Email = "joao@email.com", Password = "senha123" };
        _userManager.LoginAsync(loginParams.Email, loginParams.Password).Returns("eyJhbGciOiJIUzI1NiJ9.payload.signature");

        // Act
        var result = await _sut.Login(loginParams);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        await _userManager.Received(1).LoginAsync(loginParams.Email, loginParams.Password);
    }

    [Fact]
    public async Task Login_WhenCredentialsAreInvalid_ShouldReturnUnauthorizedWithMessage()
    {
        // Arrange
        var loginParams = new LoginParameters { Email = "invalid@email.com", Password = "wrongpassword" };
        _userManager.LoginAsync(loginParams.Email, loginParams.Password).Returns("Invalid password.");

        // Act
        var result = await _sut.Login(loginParams);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_WhenManagerReturnsNull_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginParams = new LoginParameters { Email = "joao@email.com", Password = "senha123" };
        _userManager.LoginAsync(loginParams.Email, loginParams.Password).Returns((string)null!);

        // Act
        var result = await _sut.Login(loginParams);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Create_ShouldCallManagerAndReturnCreatedWithUser()
    {
        // Arrange
        var dto = new UserCreateRequestDto { Name = "João", LastName = "Silva", Email = "joao@email.com", BirthDate = DefaultBirthDate, Password = "senha123" };
        var responseDto = new UserResponseDto { Name = "João", LastName = "Silva", Email = "joao@email.com", BirthDate = DefaultBirthDate };
        _userManager.CreateUserAsync(dto).Returns(responseDto);

        // Act
        var result = await _sut.Create(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal(responseDto, createdResult.Value);
        await _userManager.Received(1).CreateUserAsync(dto);
    }

    [Fact]
    public async Task PutAsync_WhenUserExists_ShouldSetIdAndReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UserUpdateRequestDto { Name = "Maria", LastName = "Santos", Email = "maria@email.com", BirthDate = DefaultBirthDate };
        var responseDto = new UserResponseDto { Name = "Maria", LastName = "Santos", Email = "maria@email.com", BirthDate = DefaultBirthDate };
        _userManager.UpdateUserAsync(Arg.Is<UserUpdateRequestDto>(r => r.Id == id)).Returns(responseDto);

        // Act
        var result = await _sut.PutAsync(id, dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(responseDto, okResult.Value);
        Assert.Equal(id, dto.Id);
    }

    [Fact]
    public async Task PutAsync_WhenUserNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UserUpdateRequestDto { Name = "Maria" };
        _userManager.UpdateUserAsync(Arg.Any<UserUpdateRequestDto>())
            .Returns(new UserResponseDto { Error = true, ErrorMessage = "User not found." });

        // Act
        var result = await _sut.PutAsync(id, dto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserExists_ShouldReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var responseDto = new UserResponseDto { Name = "João", Email = "joao@email.com" };
        _userManager.DeleteUser(id).Returns(responseDto);

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(responseDto, okResult.Value);
        await _userManager.Received(1).DeleteUser(id);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _userManager.DeleteUser(id)
            .Returns(new UserResponseDto { Error = true, ErrorMessage = "User not found." });

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ShouldReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var responseDto = new UserResponseDto { Name = "João", Email = "joao@email.com", BirthDate = DefaultBirthDate };
        _userManager.GetUserByIdAsync(id).Returns(responseDto);

        // Act
        var result = await _sut.GetByIdAsync(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(responseDto, okResult.Value);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _userManager.GetUserByIdAsync(id).Returns((UserResponseDto)null!);

        // Act
        var result = await _sut.GetByIdAsync(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOkWithList()
    {
        // Arrange
        var users = new List<UserResponseDto>
        {
            new() { Name = "João", Email = "joao@email.com" },
            new() { Name = "Maria", Email = "maria@email.com" }
        };
        _userManager.GetAllUsersAsync().Returns(users);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(users, okResult.Value);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoUsers_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        _userManager.GetAllUsersAsync().Returns([]);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<UserResponseDto>>(okResult.Value);
        Assert.Empty(list);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserExists_ShouldReturnOk()
    {
        // Arrange
        var email = "joao@email.com";
        var responseDto = new UserResponseDto { Name = "João", Email = email, BirthDate = DefaultBirthDate };
        _userManager.GetUserByEmailAsync(email).Returns(responseDto);

        // Act
        var result = await _sut.GetByEmailAsync(email);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(responseDto, okResult.Value);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _userManager.GetUserByEmailAsync("naoexiste@email.com").Returns((UserResponseDto)null!);

        // Act
        var result = await _sut.GetByEmailAsync("naoexiste@email.com");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
