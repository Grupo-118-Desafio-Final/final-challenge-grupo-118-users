using Domain.Plan.Ports.Out;
using Microsoft.EntityFrameworkCore;

namespace Infra.Database.SqlServer.Plan.Repositories;

public class PlanRepository : IPlanRepository
{
    private readonly AppDbContext _appDbContext;

    public PlanRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task CreateAsync(Domain.Plan.Entities.Plan plan)
    {
        await _appDbContext.AddAsync(plan);
        await _appDbContext.SaveChangesAsync();
    }

    public void Delete(Domain.Plan.Entities.Plan entity)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Domain.Plan.Entities.Plan>> GetAll()
    {
        return await _appDbContext.Plans.ToListAsync();
    }

    public async Task<Domain.Plan.Entities.Plan> GetByIdAsync(int id)
    {
        return await _appDbContext.Plans.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Domain.Plan.Entities.Plan> GetByNameAsync(string name)
    {
        return await _appDbContext.Plans.FirstOrDefaultAsync(x => x.Name == name);
    }

    public Task UpdateAsync(Domain.Plan.Entities.Plan entity)
    {
        throw new NotImplementedException();
    }
}
