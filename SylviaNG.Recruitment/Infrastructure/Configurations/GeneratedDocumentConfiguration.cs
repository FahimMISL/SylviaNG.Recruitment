using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class GeneratedDocumentConfiguration : IEntityTypeConfiguration<GeneratedDocument>
    {
        public void Configure(EntityTypeBuilder<GeneratedDocument> builder)
        {
            builder.ToTable("GeneratedDocuments");
            builder.HasKey(g => g.GeneratedDocumentId);

            builder.Property(g => g.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(g => g.FileName)
                .HasMaxLength(300);

            builder.Property(g => g.FileFormat)
                .HasMaxLength(50);

            // Indexes
            builder.HasIndex(g => g.DocumentTemplateId);
            builder.HasIndex(g => g.CandidateId);
            builder.HasIndex(g => g.JobApplicationId);

            // Relationships
            builder.HasOne(g => g.Candidate)
                .WithMany()
                .HasForeignKey(g => g.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(g => g.JobApplication)
                .WithMany()
                .HasForeignKey(g => g.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(g => g.GeneratedByUser)
                .WithMany()
                .HasForeignKey(g => g.GeneratedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(g => g.Acceptance)
                .WithOne(a => a.GeneratedDocument)
                .HasForeignKey<DocumentAcceptance>(a => a.GeneratedDocumentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
