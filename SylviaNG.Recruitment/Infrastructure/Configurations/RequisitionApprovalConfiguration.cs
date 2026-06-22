using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class RequisitionApprovalConfiguration : IEntityTypeConfiguration<RequisitionApproval>
    {
        public void Configure(EntityTypeBuilder<RequisitionApproval> builder)
        {
            builder.ToTable("RequisitionApprovals");
            builder.HasKey(a => a.RequisitionApprovalId);

            builder.Property(a => a.ApproverRole)
                .HasMaxLength(50);

            builder.Property(a => a.Action)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.Comments)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(a => a.RequisitionId);
            builder.HasIndex(a => a.ApproverUserId);

            // Relationships
            builder.HasOne(a => a.ApproverUser)
                .WithMany()
                .HasForeignKey(a => a.ApproverUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
