using Domain.Users.Ports.In;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserEntity = Domain.Users.Entities.User;

namespace Infra.Password;

public class PasswordManager : IPasswordManager
{
    private readonly IConfiguration _configuration;
    public PasswordManager(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>  
    /// Creates a hash for the specified password using HMACSHA512 and a secret key.  
    /// </summary>  
    /// <param name="password">The plain text password to hash.</param>  
    /// <param name="passwordHash">The resulting hashed password as a Base64-encoded string.</param>  
    public void CreatePasswordHash(string password, out string passwordHash)
    {
        var rawKey = _configuration["Security:Key"];
        if (string.IsNullOrWhiteSpace(rawKey))
            throw new InvalidOperationException("Security:Key não está configurado. Defina-o via variável de ambiente ou secrets.");

        var secretKey = Encoding.UTF8.GetBytes(rawKey);
        using var hmac = new HMACSHA512(secretKey);
        passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        var rawKey = _configuration["Security:Key"];
        if (string.IsNullOrWhiteSpace(rawKey))
            throw new InvalidOperationException("Security:Key não está configurado. Defina-o via variável de ambiente ou secrets.");

        var secretKey = Encoding.UTF8.GetBytes(rawKey);
        using var hmac = new HMACSHA512(secretKey);
        var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        return computedHash == storedHash;
    }

    public string GenerateJwtToken(UserEntity user)
    {
        var rawJwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(rawJwtKey) || rawJwtKey.Length < 32)
            throw new InvalidOperationException("Jwt:Key não está configurado ou é muito curto (mínimo 32 caracteres). Defina-o via variável de ambiente ou secrets.");

        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(rawJwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["Jwt:ExpirationMinutes"])
            ),
            SigningCredentials = creds,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

        return token;
    }
}
