using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class DocumentAcceptanceConfiguration : IEntityTypeConfiguration<DocumentAcceptance>
    {
        public void Configure(EntityTypeBuilder<DocumentAcceptance> builder)
        {
            builder.ToTable("DocumentAcceptances");
            builder.HasKey(a => a.DocumentAcceptanceId);

            builder.Property(a => a.AcceptanceStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.DeclineReason)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(a => a.GeneratedDocumentId).IsUnique();
            builder.HasIndex(a => a.CandidateId);

            // Relationships
            builder.HasOne(a => a.Candidate)
                .WithMany()
                .HasForeignKey(a => a.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
