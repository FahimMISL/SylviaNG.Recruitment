using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class InterviewRoundConfigPanelMemberConfiguration : IEntityTypeConfiguration<InterviewRoundConfigPanelMember>
    {
        public void Configure(EntityTypeBuilder<InterviewRoundConfigPanelMember> builder)
        {
            builder.ToTable("InterviewRoundConfigPanelMembers");
            builder.HasKey(p => new { p.InterviewRoundConfigId, p.EmployeeId });

            builder.HasOne(p => p.InterviewRoundConfig)
                .WithMany(r => r.PanelMembers)
                .HasForeignKey(p => p.InterviewRoundConfigId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Employee)
                .WithMany()
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
