using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class PasswordResetOtpConfiguration : IEntityTypeConfiguration<PasswordResetOtp>
    {
        public void Configure(EntityTypeBuilder<PasswordResetOtp> builder)
        {
            builder.ToTable("PasswordResetOtps");
            builder.HasKey(o => o.PasswordResetOtpId);

            builder.Property(o => o.KeycloakUserId).IsRequired().HasMaxLength(128);
            builder.Property(o => o.Username).IsRequired().HasMaxLength(256);
            builder.Property(o => o.OtpCodeHash).IsRequired().HasMaxLength(128);

            builder.HasIndex(o => o.ChallengeId).IsUnique();
            builder.HasIndex(o => o.KeycloakUserId);
        }
    }
}
