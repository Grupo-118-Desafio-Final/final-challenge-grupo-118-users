using Domain.Users.Ports.In;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserEntity = Domain.Users.Entities.User;

namespace Infra.Password;

public class PasswordManager : IPasswordManager
{
    private readonly SecuritySettings _settings;

    public PasswordManager(IOptions<SecuritySettings> settings)
    {
        _settings = settings.Value;
    }

    /// <summary>
    /// Creates a hash for the specified password using HMACSHA512 and a secret key.
    /// </summary>
    /// <param name="password">The plain text password to hash.</param>
    /// <param name="passwordHash">The resulting hashed password as a Base64-encoded string.</param>
    public void CreatePasswordHash(string password, out string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(_settings.SecurityKey))
            throw new InvalidOperationException("SecurityKey não está configurado.");

        var secretKey = Encoding.UTF8.GetBytes(_settings.SecurityKey);
        using var hmac = new HMACSHA512(secretKey);
        passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(_settings.SecurityKey))
            throw new InvalidOperationException("SecurityKey não está configurado.");

        var secretKey = Encoding.UTF8.GetBytes(_settings.SecurityKey);
        using var hmac = new HMACSHA512(secretKey);
        var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        return computedHash == storedHash;
    }

    public string GenerateJwtToken(UserEntity user)
    {
        if (string.IsNullOrWhiteSpace(_settings.JwtKey) || _settings.JwtKey.Length < 32)
            throw new InvalidOperationException("JwtKey não está configurado ou é muito curto (mínimo 32 caracteres).");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_settings.JwtExpirationMinutes),
            SigningCredentials = creds,
            Issuer = _settings.JwtIssuer,
            Audience = _settings.JwtAudience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }
}
