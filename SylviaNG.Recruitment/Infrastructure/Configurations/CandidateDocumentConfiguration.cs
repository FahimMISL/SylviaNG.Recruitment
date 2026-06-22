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

            builder.Property(d => d.FileName)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(d => d.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(d => d.ContentType)
                .HasMaxLength(100);

            builder.Property(d => d.FileData)
                .HasColumnType("bytea");

            // Indexes
            builder.HasIndex(d => d.CandidateId);
        }
    }
}
