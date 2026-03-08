using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiKeyEntity = Domain.ApiKey.Entities.ApiKey;

namespace Infra.Database.SqlServer.ApiKey.Configuraion;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKeyEntity>
{
    public void Configure(EntityTypeBuilder<ApiKeyEntity> builder)
    {
        builder.ToTable("ApiKeys");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
               .ValueGeneratedNever();

        builder.Property(a => a.Key)
               .IsRequired()
               .HasMaxLength(200);

        builder.HasIndex(a => a.Key)
               .IsUnique();

        builder.Property(a => a.IsActive)
               .IsRequired();

        builder.Property(a => a.CreatedAt)
               .IsRequired();
    }
}
