using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class AppointmentLetterConfiguration : IEntityTypeConfiguration<AppointmentLetter>
    {
        public void Configure(EntityTypeBuilder<AppointmentLetter> builder)
        {
            builder.ToTable("AppointmentLetters");
            builder.HasKey(a => a.AppointmentLetterId);

            builder.Property(a => a.FinalBody).IsRequired();
            builder.Property(a => a.GeneratedPdfPath).IsRequired().HasMaxLength(500);

            builder.HasIndex(a => a.JobApplicationId);
            builder.HasIndex(a => a.OfferLetterId);
            builder.HasIndex(a => a.CompanyId);

            builder.HasOne(a => a.JobApplication)
                .WithMany(j => j.AppointmentLetters)
                .HasForeignKey(a => a.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.OfferLetter)
                .WithMany(o => o.AppointmentLetters)
                .HasForeignKey(a => a.OfferLetterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.DocumentTemplate)
                .WithMany(t => t.AppointmentLetters)
                .HasForeignKey(a => a.DocumentTemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
