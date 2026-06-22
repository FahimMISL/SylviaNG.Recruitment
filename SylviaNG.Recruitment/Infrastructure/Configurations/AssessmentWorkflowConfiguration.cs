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

            builder.Property(w => w.WorkflowName)
                .IsRequired()
                .HasMaxLength(200);

            // Indexes
            builder.HasIndex(w => w.RequisitionId);

            // Relationships
            builder.HasOne(w => w.Requisition)
                .WithMany()
                .HasForeignKey(w => w.RequisitionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(w => w.Stages)
                .WithOne(s => s.AssessmentWorkflow)
                .HasForeignKey(s => s.AssessmentWorkflowId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
