using Domain.Users.Entities;
using Domain.Users.Ports.Out;
using Microsoft.EntityFrameworkCore;

namespace Infra.Database.SqlServer.Users.Repositoires;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _appDbContext;

    public UserRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task CreateAsync(User user)
    {
        await _appDbContext.Users.AddAsync(user);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task Delete(User entity)
    {
        _appDbContext.Users.Remove(entity);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<List<User>> GetAll()
    {
        return await _appDbContext.Users.ToListAsync();
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        return await _appDbContext.Users.FindAsync(id);
    }

    public async Task UpdateAsync(User entity)
    {
        _appDbContext.Users.Update(entity);
        await _appDbContext.SaveChangesAsync();
    }
}
