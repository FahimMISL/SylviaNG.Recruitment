using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class CandidateEducationConfiguration : IEntityTypeConfiguration<CandidateEducation>
    {
        public void Configure(EntityTypeBuilder<CandidateEducation> builder)
        {
            builder.ToTable("CandidateEducations");
            builder.HasKey(e => e.CandidateEducationId);

            builder.Property(e => e.Degree)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.FieldOfStudy)
                .HasMaxLength(200);

            builder.Property(e => e.Institution)
                .HasMaxLength(300);

            builder.Property(e => e.Board)
                .HasMaxLength(200);

            builder.Property(e => e.Result)
                .HasMaxLength(50);

            builder.Property(e => e.ResultScale)
                .HasMaxLength(50);

            // Indexes
            builder.HasIndex(e => e.CandidateId);
        }
    }
}
