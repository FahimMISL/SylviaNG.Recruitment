using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class CvParsingResultConfiguration : IEntityTypeConfiguration<CvParsingResult>
    {
        public void Configure(EntityTypeBuilder<CvParsingResult> builder)
        {
            builder.ToTable("CvParsingResults");
            builder.HasKey(c => c.CvParsingResultId);

            builder.Property(c => c.ParsedDataJson)
                .HasColumnType("text");

            builder.Property(c => c.ConfidenceScore)
                .HasColumnType("decimal(5,2)");

            builder.Property(c => c.ParsingErrors)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(c => c.CandidateId);
            builder.HasIndex(c => c.CandidateDocumentId);

            // Relationships
            builder.HasOne(c => c.Candidate)
                .WithMany()
                .HasForeignKey(c => c.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.CandidateDocument)
                .WithMany()
                .HasForeignKey(c => c.CandidateDocumentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
