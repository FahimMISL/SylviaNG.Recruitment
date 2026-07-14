using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ApplicationStatusHistoryConfiguration : IEntityTypeConfiguration<ApplicationStatusHistory>
    {
        public void Configure(EntityTypeBuilder<ApplicationStatusHistory> builder)
        {
            builder.ToTable("ApplicationStatusHistories");
            builder.HasKey(h => h.ApplicationStatusHistoryId);

            builder.Property(h => h.FromStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(h => h.ToStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(h => h.ChangedByUserName)
                .HasMaxLength(200);

            builder.Property(h => h.Note)
                .HasColumnType("text");

            builder.HasIndex(h => h.JobApplicationId);

            builder.HasOne(h => h.JobApplication)
                .WithMany(a => a.StatusHistory)
                .HasForeignKey(h => h.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(h => h.Reason)
                .WithMany()
                .HasForeignKey(h => h.ReasonId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
