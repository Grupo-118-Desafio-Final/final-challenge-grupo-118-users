using FinalChallengeUsers.API.Model;

namespace UnitTests.Adapters.Model;

public class CreateUserParametersTest
{
    [Fact]
    public void CreateUserParameters_Email_ShouldBeSettable()
    {
        var model = new CreateUserParameters { Email = "user@email.com" };
        Assert.Equal("user@email.com", model.Email);
    }

    [Fact]
    public void CreateUserParameters_Password_ShouldBeSettable()
    {
        var model = new CreateUserParameters { Password = "senha123" };
        Assert.Equal("senha123", model.Password);
    }

    [Fact]
    public void CreateUserParameters_Name_ShouldBeSettable()
    {
        var model = new CreateUserParameters { Name = "João" };
        Assert.Equal("João", model.Name);
    }

    [Fact]
    public void CreateUserParameters_LastName_ShouldBeSettable()
    {
        var model = new CreateUserParameters { LastName = "Silva" };
        Assert.Equal("Silva", model.LastName);
    }

    [Fact]
    public void CreateUserParameters_AllProperties_ShouldBeSettable()
    {
        var model = new CreateUserParameters
        {
            Email = "joao@email.com",
            Password = "senha@123",
            Name = "João",
            LastName = "Silva"
        };

        Assert.Equal("joao@email.com", model.Email);
        Assert.Equal("senha@123", model.Password);
        Assert.Equal("João", model.Name);
        Assert.Equal("Silva", model.LastName);
    }
}
