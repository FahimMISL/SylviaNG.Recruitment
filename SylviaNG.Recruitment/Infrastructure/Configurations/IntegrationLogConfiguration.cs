using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class IntegrationLogConfiguration : IEntityTypeConfiguration<IntegrationLog>
    {
        public void Configure(EntityTypeBuilder<IntegrationLog> builder)
        {
            builder.ToTable("IntegrationLogs");
            builder.HasKey(l => l.IntegrationLogId);

            builder.Property(l => l.IntegrationType)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(l => l.OperationName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.RequestPayloadJson)
                .HasColumnType("text");

            builder.Property(l => l.ResponsePayloadJson)
                .HasColumnType("text");

            builder.Property(l => l.ErrorMessage)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(l => l.IntegrationType);
            builder.HasIndex(l => l.ExecutedAt);
            builder.HasIndex(l => l.IsSuccess);
        }
    }
}
