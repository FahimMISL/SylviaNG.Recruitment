using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class CandidateLoginOtpConfiguration : IEntityTypeConfiguration<CandidateLoginOtp>
    {
        public void Configure(EntityTypeBuilder<CandidateLoginOtp> builder)
        {
            builder.ToTable("CandidateLoginOtps");
            builder.HasKey(o => o.CandidateLoginOtpId);

            builder.Property(o => o.Username).IsRequired().HasMaxLength(256);
            builder.Property(o => o.OtpCodeHash).IsRequired().HasMaxLength(128);

            builder.HasIndex(o => o.ChallengeId).IsUnique();
            builder.HasIndex(o => o.Username);
        }
    }
}
