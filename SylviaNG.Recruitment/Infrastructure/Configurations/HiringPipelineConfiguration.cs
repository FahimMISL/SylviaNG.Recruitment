using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class HiringPipelineConfiguration : IEntityTypeConfiguration<HiringPipeline>
    {
        public void Configure(EntityTypeBuilder<HiringPipeline> builder)
        {
            builder.ToTable("HiringPipelines");
            builder.HasKey(p => p.HiringPipelineId);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .HasColumnType("text");

            builder.HasIndex(p => p.Name);

            builder.HasMany(p => p.Stages)
                .WithOne(s => s.HiringPipeline)
                .HasForeignKey(s => s.HiringPipelineId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.JobPostings)
                .WithOne(j => j.HiringPipeline)
                .HasForeignKey(j => j.HiringPipelineId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
