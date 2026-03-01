using ApiKeyEntity = Domain.ApiKey.Entities.ApiKey;
namespace Domain.ApiKey.Ports.Out;

public interface IApiKeyRepository
{
    Task<ApiKeyEntity?> GetByKeyAsync(string key);
}
