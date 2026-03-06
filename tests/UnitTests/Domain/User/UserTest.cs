using Domain.Users.Entities;

namespace UnitTests.Domain.Users;

public class UserTest
{
    private static readonly DateTime DefaultBirthDate = new(1990, 5, 20);

    [Fact]
    public void User_Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange & Act
        var user = new User("João", "Silva", "joao@email.com", DefaultBirthDate);

        // Assert
        Assert.Equal("João", user.Name);
        Assert.Equal("Silva", user.LastName);
        Assert.Equal("joao@email.com", user.Email);
        Assert.Equal(DefaultBirthDate, user.BirthDate);
    }

    [Fact]
    public void User_Constructor_ShouldGenerateUniqueId()
    {
        // Arrange & Act
        var user1 = new User("João", "Silva", "joao@email.com", DefaultBirthDate);
        var user2 = new User("Maria", "Souza", "maria@email.com", DefaultBirthDate);

        // Assert
        Assert.NotEqual(Guid.Empty, user1.Id);
        Assert.NotEqual(Guid.Empty, user2.Id);
        Assert.NotEqual(user1.Id, user2.Id);
    }

    [Fact]
    public void User_SetPassword_ShouldUpdatePassword()
    {
        // Arrange
        var user = new User("João", "Silva", "joao@email.com", DefaultBirthDate);

        // Act
        user.SetPassword("senhaSegura123");

        // Assert
        Assert.Equal("senhaSegura123", user.Password);
    }

    [Fact]
    public void User_SetPassword_ShouldOverwritePreviousPassword()
    {
        // Arrange
        var user = new User("João", "Silva", "joao@email.com", DefaultBirthDate);
        user.SetPassword("senhaAntiga");

        // Act
        user.SetPassword("senhaNova");

        // Assert
        Assert.Equal("senhaNova", user.Password);
    }

    [Fact]
    public void User_Name_ShouldBeReadOnly()
    {
        // Arrange & Act
        var property = typeof(User).GetProperty(nameof(User.Name));

        // Assert
        Assert.NotNull(property);
        Assert.True(property.SetMethod == null || !property.SetMethod.IsPublic);
    }

    [Fact]
    public void User_LastName_ShouldBeReadOnly()
    {
        // Arrange & Act
        var property = typeof(User).GetProperty(nameof(User.LastName));

        // Assert
        Assert.NotNull(property);
        Assert.True(property.SetMethod == null || !property.SetMethod.IsPublic);
    }

    [Fact]
    public void User_Email_ShouldBeReadOnly()
    {
        // Arrange & Act
        var property = typeof(User).GetProperty(nameof(User.Email));

        // Assert
        Assert.NotNull(property);
        Assert.True(property.SetMethod == null || !property.SetMethod.IsPublic);
    }

    [Fact]
    public void User_BirthDate_ShouldBeReadOnly()
    {
        // Arrange & Act
        var property = typeof(User).GetProperty(nameof(User.BirthDate));

        // Assert
        Assert.NotNull(property);
        Assert.True(property.SetMethod == null || !property.SetMethod.IsPublic);
    }

    [Fact]
    public void User_Constructor_WithDifferentBirthDate_ShouldStoreCorrectly()
    {
        // Arrange
        var birthDate = new DateTime(2000, 12, 31);

        // Act
        var user = new User("Ana", "Costa", "ana@email.com", birthDate);

        // Assert
        Assert.Equal(birthDate, user.BirthDate);
    }
}
