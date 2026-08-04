using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class OfferLetterConfiguration : IEntityTypeConfiguration<OfferLetter>
    {
        public void Configure(EntityTypeBuilder<OfferLetter> builder)
        {
            builder.ToTable("OfferLetters");
            builder.HasKey(o => o.OfferLetterId);

            builder.Property(o => o.Designation).IsRequired().HasMaxLength(200);
            builder.Property(o => o.OfferedSalary).HasColumnType("decimal(18,2)");
            builder.Property(o => o.ReportingManager).HasMaxLength(200);
            builder.Property(o => o.GeneratedPdfPath).IsRequired().HasMaxLength(500);

            builder.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.HasIndex(o => o.JobApplicationId);

            builder.HasOne(o => o.JobApplication)
                .WithMany(j => j.OfferLetters)
                .HasForeignKey(o => o.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.DocumentTemplate)
                .WithMany(t => t.OfferLetters)
                .HasForeignKey(o => o.DocumentTemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
