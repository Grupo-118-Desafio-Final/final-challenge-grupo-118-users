using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserPlanEntity = Domain.UserPlan.Entities.UserPlan;

namespace Infra.Database.SqlServer.UserPlan.Configuration;

public class UserPlanConfiguration : IEntityTypeConfiguration<UserPlanEntity>
{
    public void Configure(EntityTypeBuilder<UserPlanEntity> builder)
    {
        builder.ToTable("UsersPlans");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .UseIdentityColumn();

        builder.Property(t => t.UserId)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(t => t.PlanId)
            .IsRequired();

        builder.HasOne(t => t.Plan)
            .WithMany(p => p.UsersPlans)
            .HasForeignKey(t => t.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSDATETIMEOFFSET()")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSDATETIMEOFFSET()")
            .ValueGeneratedOnAdd();

        builder
            .HasIndex(x => x.UserId)
            .HasDatabaseName("IDX_UsersPlans_UserId");

        builder
            .HasIndex(x => new { x.UserId, x.PlanId })
            .HasDatabaseName("IDX_UsersPlans_UserId_PlanId");
    }
}
