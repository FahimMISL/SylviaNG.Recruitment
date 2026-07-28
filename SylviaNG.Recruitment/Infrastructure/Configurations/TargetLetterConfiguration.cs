using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class TargetLetterConfiguration : IEntityTypeConfiguration<TargetLetter>
    {
        public void Configure(EntityTypeBuilder<TargetLetter> builder)
        {
            builder.ToTable("TargetLetters");
            builder.HasKey(t => t.TargetLetterId);

            builder.Property(t => t.Kpis).IsRequired();
            builder.Property(t => t.Objectives).IsRequired();
            builder.Property(t => t.FinalBody).IsRequired();
            builder.Property(t => t.GeneratedPdfPath).IsRequired().HasMaxLength(500);

            builder.HasIndex(t => t.JobApplicationId);
            builder.HasIndex(t => t.OfferLetterId);

            builder.HasOne(t => t.JobApplication)
                .WithMany(a => a.TargetLetters)
                .HasForeignKey(t => t.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.OfferLetter)
                .WithMany(o => o.TargetLetters)
                .HasForeignKey(t => t.OfferLetterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.DocumentTemplate)
                .WithMany(d => d.TargetLetters)
                .HasForeignKey(t => t.DocumentTemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
