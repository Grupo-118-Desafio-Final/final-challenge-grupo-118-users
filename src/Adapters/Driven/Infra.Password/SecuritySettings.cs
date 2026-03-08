namespace Infra.Password;

public class SecuritySettings
{
    public string SecurityKey { get; set; } = string.Empty;
    public string JwtKey { get; set; } = string.Empty;
    public string JwtIssuer { get; set; } = string.Empty;
    public string JwtAudience { get; set; } = string.Empty;
    public int JwtExpirationMinutes { get; set; } = 60;
}
