namespace Domain.Users.Entities;

public class User
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string LastName { get; private set; }

    public string Email { get; private set; }

    public DateTime BirthDate { get; private set; }
    public string Password { get; set; }

    protected User() { }

    public void SetPassword(string password)
    {
        this.Password = password;
    }

    public User(string name, string lastName, string email, DateTime birthDate)
    {
        Id = Guid.NewGuid();
        Name = name;
        LastName = lastName;
        Email = email;
        BirthDate = birthDate;
    }
}
