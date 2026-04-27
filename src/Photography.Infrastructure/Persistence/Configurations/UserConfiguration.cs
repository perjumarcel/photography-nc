using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Core.Users;

namespace Photography.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("users");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.Email).HasColumnName("email").HasMaxLength(User.MaxEmailLength).IsRequired();
        b.HasIndex(x => x.Email).IsUnique();
        b.Property(x => x.PasswordHash).HasColumnName("password_hash").IsRequired();
        b.Property(x => x.Role).HasColumnName("role").HasMaxLength(User.MaxRoleLength).IsRequired();
        b.Property(x => x.RefreshTokenHash).HasColumnName("refresh_token_hash");
        b.HasIndex(x => x.RefreshTokenHash);
        b.Property(x => x.RefreshTokenExpiresAtUtc).HasColumnName("refresh_token_expires_at_utc");
        b.Property(x => x.LastLoginAtUtc).HasColumnName("last_login_at_utc");
        b.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        b.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc");
    }
}
