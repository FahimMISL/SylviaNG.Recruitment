using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class InterviewPanelMemberConfiguration : IEntityTypeConfiguration<InterviewPanelMember>
    {
        public void Configure(EntityTypeBuilder<InterviewPanelMember> builder)
        {
            builder.ToTable("InterviewPanelMembers");
            builder.HasKey(p => new { p.InterviewId, p.EmployeeId });

            builder.HasOne(p => p.Interview)
                .WithMany(i => i.PanelMembers)
                .HasForeignKey(p => p.InterviewId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Employee)
                .WithMany()
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
