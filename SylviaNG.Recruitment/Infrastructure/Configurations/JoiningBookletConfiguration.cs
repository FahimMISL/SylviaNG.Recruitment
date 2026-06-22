using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class JoiningBookletConfiguration : IEntityTypeConfiguration<JoiningBooklet>
    {
        public void Configure(EntityTypeBuilder<JoiningBooklet> builder)
        {
            builder.ToTable("JoiningBooklets");
            builder.HasKey(j => j.JoiningBookletId);

            builder.Property(j => j.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(j => j.FileName)
                .HasMaxLength(300);

            // Indexes
            builder.HasIndex(j => j.CandidateId);
            builder.HasIndex(j => j.JobApplicationId);

            // Relationships
            builder.HasOne(j => j.Candidate)
                .WithMany()
                .HasForeignKey(j => j.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(j => j.JobApplication)
                .WithMany()
                .HasForeignKey(j => j.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(j => j.GeneratedByUser)
                .WithMany()
                .HasForeignKey(j => j.GeneratedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
