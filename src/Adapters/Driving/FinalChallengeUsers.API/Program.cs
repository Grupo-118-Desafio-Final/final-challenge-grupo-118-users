using Application.Cache;
using Application.Plans;
using Application.User;
using Application.UserPlan;
using Domain.ApiKey.Ports.Out;
using Domain.Plan.Ports.In;
using Domain.Plan.Ports.Out;
using Domain.UserPlan.Ports.In;
using Domain.UserPlan.Ports.Out;
using Domain.Users.Ports.In;
using Domain.Users.Ports.Out;
using FinalChallengeUsers.API.Middlewares;
using Infra.Database.SqlServer;
using Infra.Database.SqlServer.ApiKey.Repositories;
using Infra.Database.SqlServer.Plan.Repositories;
using Infra.Database.SqlServer.UserPlan.Repositories;
using Infra.Database.SqlServer.Users.Repositoires;
using Infra.Password;
using Microsoft.EntityFrameworkCore;
using StandardDependencies.Injection;
using StandardDependencies.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    // using System.Reflection;
//    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
//});

var swaggerOptions = builder
    .Configuration
    .GetSection(SwaggerOptions.SectionName)
    .Get<SwaggerOptions>();

var openTelemetryOptions = builder
    .Configuration
    .GetSection(OpenTelemetryOptions.SectionName)
    .Get<OpenTelemetryOptions>();

// Configura elementos comuns: Environment Variables, OpenTelemetry e Swagger
builder.ConfigureCommonElements(openTelemetryOptions, swaggerOptions);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<SecuritySettings>(
    builder.Configuration.GetSection("Security"));

builder.Services.AddScoped<IPasswordManager, PasswordManager>();

builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IPlanManager, PlanManager>();

builder.Services.AddScoped<IUserPlanRepository, UserPlanRepository>();
builder.Services.AddScoped<IUserPlanManager, UserPlanManager>();

builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
builder.Services.AddScoped<AuthenticationMiddleware>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<CacheService>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "FinalChallengeUsersAPI";
});

builder.Services.AddHealthChecks();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.Use(async (context, next) =>
{
    var authMiddleware = context.RequestServices.GetRequiredService<AuthenticationMiddleware>();
    await authMiddleware.InvokeAsync(context, next);
});

app.UseStandarizedSwagger(swaggerOptions);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();