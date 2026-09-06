using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class OfficeNoteConfiguration : IEntityTypeConfiguration<OfficeNote>
    {
        public void Configure(EntityTypeBuilder<OfficeNote> builder)
        {
            builder.ToTable("OfficeNotes");
            builder.HasKey(o => o.OfficeNoteId);

            builder.Property(o => o.Remarks).HasMaxLength(2000);
            builder.Property(o => o.EnclosuresSummary).IsRequired().HasMaxLength(500);
            builder.Property(o => o.RenderedBody).IsRequired();
            builder.Property(o => o.GeneratedPdfPath).IsRequired().HasMaxLength(500);

            builder.HasIndex(o => o.JobApplicationId);
            builder.HasIndex(o => o.CompanyId);

            builder.HasOne(o => o.JobApplication)
                .WithMany(a => a.OfficeNotes)
                .HasForeignKey(o => o.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.DocumentTemplate)
                .WithMany(t => t.OfficeNotes)
                .HasForeignKey(o => o.DocumentTemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
