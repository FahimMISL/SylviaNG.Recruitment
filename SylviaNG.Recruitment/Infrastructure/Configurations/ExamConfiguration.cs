using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ExamConfiguration : IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> builder)
        {
            builder.ToTable("Exams");
            builder.HasKey(e => e.ExamId);

            builder.Property(e => e.ExamTitle)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.ExamStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(e => e.TotalMarks)
                .HasColumnType("decimal(7,2)");

            builder.Property(e => e.PassMarks)
                .HasColumnType("decimal(7,2)");

            builder.Property(e => e.Instructions)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(e => e.RequisitionId);
            builder.HasIndex(e => e.AssessmentStageId);
            builder.HasIndex(e => e.ExamStatus);

            // Relationships
            builder.HasOne(e => e.Requisition)
                .WithMany()
                .HasForeignKey(e => e.RequisitionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.AssessmentStage)
                .WithMany()
                .HasForeignKey(e => e.AssessmentStageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.ExamCandidates)
                .WithOne(ec => ec.Exam)
                .HasForeignKey(ec => ec.ExamId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
