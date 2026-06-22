using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class AdmitCardConfiguration : IEntityTypeConfiguration<AdmitCard>
    {
        public void Configure(EntityTypeBuilder<AdmitCard> builder)
        {
            builder.ToTable("AdmitCards");
            builder.HasKey(a => a.AdmitCardId);

            builder.Property(a => a.FileUrl)
                .HasMaxLength(500);

            builder.Property(a => a.DeliveryChannel)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.DeliveryStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.FailureReason)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(a => a.ExamCandidateId);

            // Relationships
            builder.HasOne(a => a.ExamCandidate)
                .WithMany()
                .HasForeignKey(a => a.ExamCandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.ExamSeatPlan)
                .WithMany()
                .HasForeignKey(a => a.ExamSeatPlanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
