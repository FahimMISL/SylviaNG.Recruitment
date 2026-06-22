using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class InterviewScorecardCriteriaConfiguration : IEntityTypeConfiguration<InterviewScorecardCriteria>
    {
        public void Configure(EntityTypeBuilder<InterviewScorecardCriteria> builder)
        {
            builder.ToTable("InterviewScorecardCriteria");
            builder.HasKey(c => c.InterviewScorecardCriteriaId);

            builder.Property(c => c.CriteriaName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            builder.Property(c => c.Weight)
                .HasColumnType("decimal(5,2)");

            // Indexes
            builder.HasIndex(c => c.InterviewScorecardId);
        }
    }
}
