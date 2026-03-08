namespace Domain.ApiKey.Entities;

public class ApiKey
{
    public Guid Id { get; private set; }
    public string Key { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected ApiKey() { }

    public ApiKey(string key, bool isActive = true)
    {
        Id = Guid.NewGuid();
        Key = key;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
    }
}
