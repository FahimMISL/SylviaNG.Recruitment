using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class RequisitionConfiguration : IEntityTypeConfiguration<Requisition>
    {
        public void Configure(EntityTypeBuilder<Requisition> builder)
        {
            builder.ToTable("Requisitions");
            builder.HasKey(r => r.RequisitionId);

            builder.Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.JobDescription)
                .HasColumnType("text");

            builder.Property(r => r.Justification)
                .HasColumnType("text");

            builder.Property(r => r.RequisitionType)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(r => r.RequisitionStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(r => r.NumberOfPositions)
                .HasDefaultValue(1);

            builder.Property(r => r.BudgetCode)
                .HasMaxLength(50);

            builder.Property(r => r.RoleCategory)
                .HasMaxLength(100);

            builder.Property(r => r.ReplacementEmployeeName)
                .HasMaxLength(200);

            // Indexes
            builder.HasIndex(r => r.SiteId);
            builder.HasIndex(r => r.RequisitionStatus);
            builder.HasIndex(r => r.RequestedByUserId);

            // Relationships
            builder.HasOne(r => r.RequestedByUser)
                .WithMany()
                .HasForeignKey(r => r.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.Attachments)
                .WithOne(a => a.Requisition)
                .HasForeignKey(a => a.RequisitionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Approvals)
                .WithOne(a => a.Requisition)
                .HasForeignKey(a => a.RequisitionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.StageConfigs)
                .WithOne(s => s.Requisition)
                .HasForeignKey(s => s.RequisitionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.JobPostings)
                .WithOne(j => j.Requisition)
                .HasForeignKey(j => j.RequisitionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
