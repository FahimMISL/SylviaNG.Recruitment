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

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.ExamType)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(e => e.TotalMarks).HasColumnType("decimal(6,2)");
            builder.Property(e => e.PassMarks).HasColumnType("decimal(6,2)");

            builder.HasOne(e => e.JobPosting)
                .WithMany()
                .HasForeignKey(e => e.JobPostingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.ExamVenue)
                .WithMany()
                .HasForeignKey(e => e.ExamVenueId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.QuestionGroup)
                .WithMany()
                .HasForeignKey(e => e.QuestionGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.JobPostingId);
            builder.HasIndex(e => e.ExamVenueId);
            builder.HasIndex(e => e.QuestionGroupId);
        }
    }
}
