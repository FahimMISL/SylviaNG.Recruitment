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
            builder.HasKey(l => l.ImpersonationLogId);

            builder.Property(l => l.HttpMethod).IsRequired().HasMaxLength(10);
            builder.Property(l => l.Path).IsRequired().HasMaxLength(500);

            builder.HasOne(l => l.Session)
                .WithMany(s => s.Logs)
                .HasForeignKey(l => l.ImpersonationSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
