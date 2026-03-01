using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanEntity = Domain.Plan.Entities.Plan;
namespace Infra.Database.SqlServer.Plan.Configuration;

public class PlanConfiguration : IEntityTypeConfiguration<PlanEntity>
{
    public void Configure(EntityTypeBuilder<PlanEntity> builder)
    {
        builder.ToTable("Plans");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .UseIdentityColumn();

        builder.Property(t => t.Name)
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(t => t.Price)
            .IsRequired();

        builder.Property(t => t.ImageQuality)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(t => t.MaxSizeInMegaBytes)
            .IsRequired();

        builder.Property(t => t.MaxDurationInSeconds)
            .IsRequired();

        builder.Property(t => t.Threads)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSDATETIMEOFFSET()")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.UpdatedAt)
            .HasDefaultValueSql("SYSDATETIMEOFFSET()")
            .ValueGeneratedOnUpdate();

        builder.HasIndex(t => t.Name)
            .IsUnique()
            .HasDatabaseName("IDX_Plans_Name");
    }
}
