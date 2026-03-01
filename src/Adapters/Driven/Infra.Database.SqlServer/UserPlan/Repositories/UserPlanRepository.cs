using Domain.UserPlan.Ports.Out;
using Microsoft.EntityFrameworkCore;

namespace Infra.Database.SqlServer.UserPlan.Repositories;

public class UserPlanRepository : IUserPlanRepository
{
    private readonly AppDbContext _appDbContext;

    public UserPlanRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task CreateAsync(Domain.UserPlan.Entities.UserPlan userPlan)
    {
        await _appDbContext.AddAsync(userPlan);
        await _appDbContext.SaveChangesAsync();
    }

    public void Delete(Domain.UserPlan.Entities.UserPlan userPlan)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Domain.UserPlan.Entities.UserPlan>> GetAllAsync()
    {
        return await _appDbContext.UserPlans.ToListAsync();
    }

    public async Task<Domain.UserPlan.Entities.UserPlan> GetByIdAsync(int id)
    {
        return await _appDbContext.UserPlans.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Domain.UserPlan.Entities.UserPlan> GetByUserId(string userId)
    {
        return await _appDbContext.UserPlans.FirstAsync(x => x.UserId == userId);
    }

    public Task UpdateAsync(Domain.UserPlan.Entities.UserPlan userPlan)
    {
        throw new NotImplementedException();
    }
}
