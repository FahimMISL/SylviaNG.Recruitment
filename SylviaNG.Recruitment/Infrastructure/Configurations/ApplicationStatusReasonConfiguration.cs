using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ApplicationStatusReasonConfiguration : IEntityTypeConfiguration<ApplicationStatusReason>
    {
        public void Configure(EntityTypeBuilder<ApplicationStatusReason> builder)
        {
            builder.ToTable("ApplicationStatusReasons");
            builder.HasKey(r => r.ApplicationStatusReasonId);

            builder.Property(r => r.Label)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.AppliesToStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.HasIndex(r => r.AppliesToStatus);
        }
    }
}
