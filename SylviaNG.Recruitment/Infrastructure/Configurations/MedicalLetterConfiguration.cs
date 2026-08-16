using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class MedicalLetterConfiguration : IEntityTypeConfiguration<MedicalLetter>
    {
        public void Configure(EntityTypeBuilder<MedicalLetter> builder)
        {
            builder.ToTable("MedicalLetters");
            builder.HasKey(m => m.MedicalLetterId);

            builder.Property(m => m.MedicalTestCenter).IsRequired().HasMaxLength(300);
            builder.Property(m => m.RequiredTests).IsRequired();
            builder.Property(m => m.FinalBody).IsRequired();
            builder.Property(m => m.GeneratedPdfPath).IsRequired().HasMaxLength(500);

            builder.HasIndex(m => m.JobApplicationId);
            builder.HasIndex(m => m.OfferLetterId);
            builder.HasIndex(m => m.CompanyId);

            builder.HasOne(m => m.JobApplication)
                .WithMany(a => a.MedicalLetters)
                .HasForeignKey(m => m.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.OfferLetter)
                .WithMany(o => o.MedicalLetters)
                .HasForeignKey(m => m.OfferLetterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.DocumentTemplate)
                .WithMany(t => t.MedicalLetters)
                .HasForeignKey(m => m.DocumentTemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
