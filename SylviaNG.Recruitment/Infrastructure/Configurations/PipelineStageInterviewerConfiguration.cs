using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class PipelineStageInterviewerConfiguration : IEntityTypeConfiguration<PipelineStageInterviewer>
    {
        public void Configure(EntityTypeBuilder<PipelineStageInterviewer> builder)
        {
            builder.ToTable("PipelineStageInterviewers");
            builder.HasKey(i => new { i.PipelineStageId, i.EmployeeId });

            builder.HasOne(i => i.Employee)
                .WithMany()
                .HasForeignKey(i => i.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
