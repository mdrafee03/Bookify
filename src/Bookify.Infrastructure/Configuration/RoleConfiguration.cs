using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configuration;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(role => role.Id);

        builder
            .Property(role => role.Name)
            .HasMaxLength(20)
            .HasConversion(name => name.Value.ToString(), value => RoleName.FromString(value));

        builder.HasMany(role => role.Users).WithMany(user => user.UserRoles);

        builder.HasData([Role.Registered, Role.Admin]);
    }
}
