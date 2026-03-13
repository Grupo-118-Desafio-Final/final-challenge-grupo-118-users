using Microsoft.AspNetCore.Mvc;
using UserEntity = Domain.Users.Entities.User;
using PlanEntity = Domain.Plan.Entities.Plan;
namespace Domain.Users.Ports.In;

public interface IPasswordManager
{
    /// <summary>
    /// Creates a hash for the specified password.
    /// </summary>
    /// <param name="password">The plain text password to hash.</param>
    /// <param name="passwordHash">The resulting hashed password.</param>
    void CreatePasswordHash(string password, out string passwordHash);

    bool VerifyPassword(string password, string storedHash);

    string GenerateJwtToken(UserEntity user, PlanEntity plan);
}
