using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class UserInviteOtpConfiguration : IEntityTypeConfiguration<UserInviteOtp>
    {
        public void Configure(EntityTypeBuilder<UserInviteOtp> builder)
        {
            builder.ToTable("UserInviteOtps");
            builder.HasKey(o => o.UserInviteOtpId);

            builder.Property(o => o.Email).IsRequired().HasMaxLength(256);
            builder.Property(o => o.OtpCodeHash).IsRequired().HasMaxLength(128);

            builder.HasIndex(o => o.ChallengeId).IsUnique();
            builder.HasIndex(o => o.Email);
        }
    }
}
