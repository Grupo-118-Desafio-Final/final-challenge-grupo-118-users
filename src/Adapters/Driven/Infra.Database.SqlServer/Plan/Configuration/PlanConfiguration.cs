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

        builder.HasData(GetPreconfiguredPlans());
    }


    private IEnumerable<PlanEntity> GetPreconfiguredPlans()
    {
        return new List<PlanEntity>
        {
            new PlanEntity
            {
                Id = 1,
                Name = "Default",
                Price = 9.99m,
                ImageQuality = Domain.Plan.ValueObjects.ImageQualityEnum.Hd,
                MaxSizeInMegaBytes = "200",
                MaxDurationInSeconds = "20",
                Threads = "2"
            },
            new PlanEntity
            {
                Id = 2,
                Name = "Standard",
                Price = 19.99m,
                ImageQuality = Domain.Plan.ValueObjects.ImageQualityEnum.FullHd,
                MaxSizeInMegaBytes = "2000",
                MaxDurationInSeconds = "1200",
                Threads = "4"
            },
            new PlanEntity
            {
                Id = 3,
                Name = "Premium",
                Price = 29.99m,
                ImageQuality = Domain.Plan.ValueObjects.ImageQualityEnum.FourK,
                MaxSizeInMegaBytes = "10000",
                MaxDurationInSeconds = "3600",
                Threads = "8"
            }
        };
    }
}
