using Infra.Database.SqlServer.Plan.Configuration;
using Infra.Database.SqlServer.UserPlan.Configuration;
using Microsoft.EntityFrameworkCore;
using PlanEntity = Domain.Plan.Entities.Plan;
using UserPlanEntity = Domain.UserPlan.Entities.UserPlan;
using ApiKeyEntity = Domain.ApiKey.Entities.ApiKey;
using UserEntity = Domain.Users.Entities.User;
using Infra.Database.SqlServer.ApiKey.Configuraion;
using Infra.Database.SqlServer.Users.Configuration;

namespace Infra.Database.SqlServer
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<PlanEntity> Plans { get; set; }
        public DbSet<UserPlanEntity> UserPlans { get; set; }
        public DbSet<ApiKeyEntity> ApiKeys { get; set; }
        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PlanConfiguration());
            modelBuilder.ApplyConfiguration(new UserPlanConfiguration());
            modelBuilder.ApplyConfiguration(new ApiKeyConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
