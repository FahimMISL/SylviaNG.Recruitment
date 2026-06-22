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

            builder.Property(h => h.VenueName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(h => h.Address)
                .HasMaxLength(500);

            builder.Property(h => h.VirtualLink)
                .HasMaxLength(500);

            builder.Property(h => h.ContactPerson)
                .HasMaxLength(200);

            builder.Property(h => h.ContactPhone)
                .HasMaxLength(50);

            // Relationships
            builder.HasMany(h => h.SeatPlans)
                .WithOne(sp => sp.ExamHall)
                .HasForeignKey(sp => sp.ExamHallId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
