using Domain.Users.Entities;
using Infra.Database.SqlServer.Users.Repositoires;

namespace UnitTests.Infra.Database;

public class UserRepositoryTest : InfraTestBase
{
    private readonly UserRepository _sut;
    private static readonly DateTime DefaultBirthDate = new(1990, 5, 20);

    public UserRepositoryTest()
    {
        _sut = new UserRepository(DbContext);
    }

    private static User CreateUser(string name, string lastName, string email)
    {
        var user = new User(name, lastName, email, DefaultBirthDate);
        user.SetPassword("senha-hash-padrao");
        return user;
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistUserInDatabase()
    {
        // Arrange
        var user = CreateUser("João", "Silva", "joao@email.com");

        // Act
        await _sut.CreateAsync(user);

        // Assert
        var found = await DbContext.Users.FindAsync(user.Id);
        Assert.NotNull(found);
        Assert.Equal("João", found!.Name);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllUsers()
    {
        // Arrange
        await _sut.CreateAsync(CreateUser("User1", "Last1", "u1@email.com"));
        await _sut.CreateAsync(CreateUser("User2", "Last2", "u2@email.com"));

        // Act
        var result = await _sut.GetAll();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAll_WhenEmpty_ShouldReturnEmptyList()
    {
        // Act
        var result = await _sut.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var user = CreateUser("Maria", "Costa", "maria@email.com");
        await _sut.CreateAsync(user);

        // Act
        var result = await _sut.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal("Maria", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserNotFound_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var user = CreateUser("Pedro", "Lima", "pedro@email.com");
        await _sut.CreateAsync(user);

        // Act
        var result = await _sut.GetByEmailAsync("pedro@email.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("pedro@email.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenEmailNotFound_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetByEmailAsync("naoexiste@email.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        // Arrange
        var user = CreateUser("Antigo", "Nome", "antigo@email.com");
        await _sut.CreateAsync(user);

        // EF Core tracks the entity; to simulate update we modify via context directly
        var tracked = await DbContext.Users.FindAsync(user.Id);
        tracked!.SetPassword("nova-senha");

        // Act
        await _sut.UpdateAsync(tracked);

        // Assert
        var updated = await DbContext.Users.FindAsync(user.Id);
        Assert.Equal("nova-senha", updated!.Password);
    }

    [Fact]
    public async Task Delete_ShouldRemoveUserFromDatabase()
    {
        // Arrange
        var user = CreateUser("Deletar", "User", "deletar@email.com");
        await _sut.CreateAsync(user);

        // Act
        var tracked = await DbContext.Users.FindAsync(user.Id);
        await _sut.Delete(tracked!);

        // Assert
        var found = await DbContext.Users.FindAsync(user.Id);
        Assert.Null(found);
    }
}
