using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class DashboardWidgetConfiguration : IEntityTypeConfiguration<DashboardWidget>
    {
        public void Configure(EntityTypeBuilder<DashboardWidget> builder)
        {
            builder.ToTable("DashboardWidgets");
            builder.HasKey(w => w.DashboardWidgetId);

            builder.Property(w => w.WidgetType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(w => w.Title)
                .HasMaxLength(200);

            builder.Property(w => w.ConfigJson)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(w => w.UserId);

            // Relationships
            builder.HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
