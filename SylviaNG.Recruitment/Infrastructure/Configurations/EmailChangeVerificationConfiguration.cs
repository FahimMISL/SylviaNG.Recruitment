using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class EmailChangeVerificationConfiguration : IEntityTypeConfiguration<EmailChangeVerification>
    {
        public void Configure(EntityTypeBuilder<EmailChangeVerification> builder)
        {
            builder.ToTable("EmailChangeVerifications");
            builder.HasKey(o => o.EmailChangeVerificationId);

            builder.Property(o => o.KeycloakUserId).IsRequired().HasMaxLength(128);
            builder.Property(o => o.NewEmail).IsRequired().HasMaxLength(200);
            builder.Property(o => o.OtpCodeHash).IsRequired().HasMaxLength(128);

            builder.HasIndex(o => o.ChallengeId).IsUnique();
            builder.HasIndex(o => o.KeycloakUserId);
        }
    }
}
