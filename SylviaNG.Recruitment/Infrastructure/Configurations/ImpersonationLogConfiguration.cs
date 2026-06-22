using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ImpersonationLogConfiguration : IEntityTypeConfiguration<ImpersonationLog>
    {
        public void Configure(EntityTypeBuilder<ImpersonationLog> builder)
        {
            builder.ToTable("ImpersonationLogs");
            builder.HasKey(i => i.ImpersonationLogId);

            builder.Property(i => i.Reason)
                .IsRequired()
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(i => i.AdminUserId);
            builder.HasIndex(i => i.CandidateId);

            // Relationships
            builder.HasOne(i => i.AdminUser)
                .WithMany()
                .HasForeignKey(i => i.AdminUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Candidate)
                .WithMany()
                .HasForeignKey(i => i.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
