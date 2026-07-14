using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class CandidateDocumentConfiguration : IEntityTypeConfiguration<CandidateDocument>
    {
        public void Configure(EntityTypeBuilder<CandidateDocument> builder)
        {
            builder.ToTable("CandidateDocuments");
            builder.HasKey(d => d.CandidateDocumentId);

            builder.Property(d => d.DocumentType)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(d => d.FileName).IsRequired().HasMaxLength(255);
            builder.Property(d => d.StoredFileName).IsRequired().HasMaxLength(255);
            builder.Property(d => d.FilePath).IsRequired().HasMaxLength(500);
            builder.Property(d => d.ContentType).IsRequired().HasMaxLength(100);

            builder.HasIndex(d => d.CandidateProfileId);

            builder.HasOne(d => d.CandidateProfile)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
