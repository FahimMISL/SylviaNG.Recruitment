using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class EmailOtpConfiguration : IEntityTypeConfiguration<EmailOtp>
    {
        public void Configure(EntityTypeBuilder<EmailOtp> builder)
        {
            builder.ToTable("EmailOtps");
            builder.HasKey(e => e.EmailOtpId);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(e => e.OtpCode)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasIndex(e => new { e.Email, e.IsUsed });
        }
    }
}
