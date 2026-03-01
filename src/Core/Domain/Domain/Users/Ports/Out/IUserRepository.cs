using Domain.Users.Entities;

namespace Domain.Users.Ports.Out;

public interface IUserRepository
{
    Task CreateAsync(User user);
    Task UpdateAsync(User entity);
    Task Delete(User entity);
    Task<User> GetByIdAsync(Guid id);
    Task<List<User>> GetAll();
    Task<User> GetByEmailAsync(string email);
}
