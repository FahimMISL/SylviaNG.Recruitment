using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class JoiningBookletConfiguration : IEntityTypeConfiguration<JoiningBooklet>
    {
        public void Configure(EntityTypeBuilder<JoiningBooklet> builder)
        {
            builder.ToTable("JoiningBooklets");
            builder.HasKey(j => j.JoiningBookletId);

            builder.Property(j => j.BatchLabel).IsRequired().HasMaxLength(200);
            builder.Property(j => j.RenderedBody).IsRequired();
            builder.Property(j => j.GeneratedPdfPath).IsRequired().HasMaxLength(500);

            builder.HasIndex(j => j.JobApplicationId);
            builder.HasIndex(j => j.OfferLetterId);
            builder.HasIndex(j => j.CompanyId);

            builder.HasOne(j => j.JobApplication)
                .WithMany(a => a.JoiningBooklets)
                .HasForeignKey(j => j.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(j => j.OfferLetter)
                .WithMany(o => o.JoiningBooklets)
                .HasForeignKey(j => j.OfferLetterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(j => j.DocumentTemplate)
                .WithMany(t => t.JoiningBooklets)
                .HasForeignKey(j => j.DocumentTemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
