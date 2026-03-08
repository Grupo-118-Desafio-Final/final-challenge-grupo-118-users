using Domain.ApiKey.Ports.Out;
using Infra.Password;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinalChallengeUsers.API.Middlewares;

public class AuthenticationMiddleware
{
    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly SecuritySettings _securitySettings;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationMiddleware> _logger;
    private static readonly string[] AllowedPaths = new[] { "/users/login", "/users","health","/swagger" };

    public AuthenticationMiddleware(
        IApiKeyRepository apiKeyRepository,
        ILogger<AuthenticationMiddleware> logger,
        IOptions<SecuritySettings> securitySettings,
        IConfiguration configuration)
    {
        _apiKeyRepository = apiKeyRepository;
        _logger = logger;
        _securitySettings = securitySettings.Value;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if(AllowedPaths.Any(p => context.Request.Path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase)))
        {
            await next(context);
            return;
        }

        // 1) Checa X-Api-Key
        var apiKeyHeader = context.Request.Headers["X-Api-Key"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(apiKeyHeader))
        {
            try
            {
                var apiKey = await _apiKeyRepository.GetByKeyAsync(apiKeyHeader);
                if (apiKey is not null)
                {
                    // Marca contexto como autenticado por API Key
                    context.Items["ApiKeyId"] = apiKey.Id;
                    await next(context);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar X-Api-Key");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Internal Server Error");
                return;
            }
        }

        // 2) Se não houver API key válida, tenta validar JWT
        var bearer = context.Request.Headers["Authorization"].FirstOrDefault();
        var token = bearer?.Split(" ").Last();
        if (!string.IsNullOrWhiteSpace(token))
        {
            var validIssuer = _securitySettings.JwtIssuer;
            var validAudience = _securitySettings.JwtAudience;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_securitySettings.JwtKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = !string.IsNullOrWhiteSpace(validIssuer),
                ValidIssuer = validIssuer,
                ValidateAudience = !string.IsNullOrWhiteSpace(validAudience),
                ValidAudience = validAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2)
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                if (principal?.Identity?.IsAuthenticated == true)
                {
                    // Define o ClaimsPrincipal no HttpContext para permitir [Authorize] / códigos que leem Claims
                    context.User = principal;

                    // Opcional: extrair user id do claim "sub" ou ClaimTypes.NameIdentifier
                    var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                 ?? principal.FindFirst("sub")?.Value;
                    if (!string.IsNullOrWhiteSpace(userId))
                    {
                        context.Items["UserId"] = userId;
                    }

                    await next(context);
                    return;
                }
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Token JWT inválido");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar token JWT");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Internal Server Error");
                return;
            }
        }

        // 3) Nenhuma autenticação válida
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Unauthorized");
    }
}
