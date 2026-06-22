using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ExamCandidateConfiguration : IEntityTypeConfiguration<ExamCandidate>
    {
        public void Configure(EntityTypeBuilder<ExamCandidate> builder)
        {
            builder.ToTable("ExamCandidates");
            builder.HasKey(ec => ec.ExamCandidateId);

            builder.Property(ec => ec.ObtainedMarks)
                .HasColumnType("decimal(7,2)");

            // Indexes
            builder.HasIndex(ec => ec.ExamId);
            builder.HasIndex(ec => ec.JobApplicationId);
            builder.HasIndex(ec => new { ec.ExamId, ec.JobApplicationId }).IsUnique();

            // Relationships
            builder.HasOne(ec => ec.JobApplication)
                .WithMany()
                .HasForeignKey(ec => ec.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(ec => ec.Answers)
                .WithOne(a => a.ExamCandidate)
                .HasForeignKey(a => a.ExamCandidateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
