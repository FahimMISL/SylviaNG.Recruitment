using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class CandidateRecommendationConfiguration : IEntityTypeConfiguration<CandidateRecommendation>
    {
        public void Configure(EntityTypeBuilder<CandidateRecommendation> builder)
        {
            builder.ToTable("CandidateRecommendations");
            builder.HasKey(r => r.CandidateRecommendationId);

            builder.Property(r => r.Justification)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(r => r.RecommendedByUserName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(r => r.ReviewComments)
                .HasColumnType("text");

            builder.Property(r => r.ReviewedByUserName)
                .HasMaxLength(200);

            builder.HasIndex(r => new { r.JobApplicationId, r.Status });

            builder.HasOne(r => r.JobApplication)
                .WithMany()
                .HasForeignKey(r => r.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
