using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ExamHallConfiguration : IEntityTypeConfiguration<ExamHall>
    {
        public void Configure(EntityTypeBuilder<ExamHall> builder)
        {
            builder.ToTable("ExamHalls");
            builder.HasKey(h => h.ExamHallId);

            builder.Property(h => h.HallName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(h => h.Location)
                .IsRequired()
                .HasMaxLength(300);

            builder.HasIndex(h => h.HallName);

            builder.HasMany(h => h.Invigilators)
                .WithOne(i => i.ExamHall)
                .HasForeignKey(i => i.ExamHallId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
