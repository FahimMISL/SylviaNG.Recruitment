using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ExamSeatPlanConfiguration : IEntityTypeConfiguration<ExamSeatPlan>
    {
        public void Configure(EntityTypeBuilder<ExamSeatPlan> builder)
        {
            builder.ToTable("ExamSeatPlans");
            builder.HasKey(sp => sp.ExamSeatPlanId);

            builder.Property(sp => sp.RoomNumber)
                .HasMaxLength(50);

            builder.Property(sp => sp.SeatNumber)
                .HasMaxLength(50);

            // Indexes
            builder.HasIndex(sp => sp.ExamId);
            builder.HasIndex(sp => sp.ExamHallId);
            builder.HasIndex(sp => sp.ExamCandidateId);

            // Relationships
            builder.HasOne(sp => sp.Exam)
                .WithMany()
                .HasForeignKey(sp => sp.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sp => sp.ExamCandidate)
                .WithMany()
                .HasForeignKey(sp => sp.ExamCandidateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
