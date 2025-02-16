using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configuration;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(permission => permission.Id);

        builder.Property(permission => permission.Name).HasMaxLength(20);

        builder.HasMany(permission => permission.Users).WithMany(role => role.Permissions);

        builder.HasData(Permission.UsersRead);
    }
}
