using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Database.SqlServer.Users.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
               .ValueGeneratedNever();

        builder.Property(u => u.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.LastName)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(200);

        builder.HasIndex(u => u.Email)
               .IsUnique();

        builder.Property(u => u.BirthDate)
               .IsRequired();
    }
}
