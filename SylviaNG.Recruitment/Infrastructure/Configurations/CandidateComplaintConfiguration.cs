using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class CandidateComplaintConfiguration : IEntityTypeConfiguration<CandidateComplaint>
    {
        public void Configure(EntityTypeBuilder<CandidateComplaint> builder)
        {
            builder.ToTable("CandidateComplaints");
            builder.HasKey(c => c.CandidateComplaintId);

            builder.Property(c => c.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(c => c.ComplaintStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(c => c.ResolutionNotes)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(c => c.CandidateId);
            builder.HasIndex(c => c.ComplaintStatus);

            // Relationships
            builder.HasOne(c => c.Candidate)
                .WithMany()
                .HasForeignKey(c => c.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.JobApplication)
                .WithMany()
                .HasForeignKey(c => c.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.AssignedToUser)
                .WithMany()
                .HasForeignKey(c => c.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
