using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class AssessmentWorkflowConfiguration : IEntityTypeConfiguration<AssessmentWorkflow>
    {
        public void Configure(EntityTypeBuilder<AssessmentWorkflow> builder)
        {
            builder.ToTable("AssessmentWorkflows");
            builder.HasKey(w => w.AssessmentWorkflowId);

            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(w => w.Description)
                .HasColumnType("text");

            builder.HasIndex(w => w.Name);

            builder.HasMany(w => w.Stages)
                .WithOne(s => s.AssessmentWorkflow)
                .HasForeignKey(s => s.AssessmentWorkflowId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(w => w.JobPostings)
                .WithOne(j => j.AssessmentWorkflow)
                .HasForeignKey(j => j.AssessmentWorkflowId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
