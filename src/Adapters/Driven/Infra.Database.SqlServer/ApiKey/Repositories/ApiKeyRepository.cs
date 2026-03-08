using Domain.ApiKey.Ports.Out;
using Microsoft.EntityFrameworkCore;
using ApiKeyEntity = Domain.ApiKey.Entities.ApiKey;

namespace Infra.Database.SqlServer.ApiKey.Repositories;

public class ApiKeyRepository : IApiKeyRepository
{
    private readonly AppDbContext _appDbContext;

    public ApiKeyRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<ApiKeyEntity?> GetByKeyAsync(string key)
    {
        return await _appDbContext.ApiKeys
                        .AsNoTracking()
                        .FirstOrDefaultAsync(a => a.Key == key && a.IsActive);
    }
}
