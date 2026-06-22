using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ReferralConfiguration : IEntityTypeConfiguration<Referral>
    {
        public void Configure(EntityTypeBuilder<Referral> builder)
        {
            builder.ToTable("Referrals");
            builder.HasKey(r => r.ReferralId);

            builder.Property(r => r.Source)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(r => r.ReferralStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(r => r.ReferrerName)
                .HasMaxLength(200);

            builder.Property(r => r.ReferrerContact)
                .HasMaxLength(200);

            // Indexes
            builder.HasIndex(r => r.CandidateId);
            builder.HasIndex(r => r.JobPostingId);
            builder.HasIndex(r => r.ReferralStatus);

            // Relationships
            builder.HasOne(r => r.Candidate)
                .WithMany()
                .HasForeignKey(r => r.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.JobPosting)
                .WithMany()
                .HasForeignKey(r => r.JobPostingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ReferrerEmployee)
                .WithMany()
                .HasForeignKey(r => r.ReferrerEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
