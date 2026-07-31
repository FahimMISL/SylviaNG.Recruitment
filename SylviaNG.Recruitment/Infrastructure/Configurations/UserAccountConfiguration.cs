using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
    {
        public void Configure(EntityTypeBuilder<UserAccount> builder)
        {
            builder.ToTable("UserAccounts");
            builder.HasKey(u => u.UserAccountId);

            builder.Property(u => u.KeycloakUserId).IsRequired().HasMaxLength(64);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
            builder.Property(u => u.FullName).IsRequired().HasMaxLength(200);

            builder.HasIndex(u => u.KeycloakUserId).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();
        }
    }
}
