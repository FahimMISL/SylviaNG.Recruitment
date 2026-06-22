using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ApplicationDuplicateConfiguration : IEntityTypeConfiguration<ApplicationDuplicate>
    {
        public void Configure(EntityTypeBuilder<ApplicationDuplicate> builder)
        {
            builder.ToTable("ApplicationDuplicates");
            builder.HasKey(d => d.ApplicationDuplicateId);

            builder.Property(d => d.MatchField)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Resolution)
                .HasConversion<string>()
                .HasMaxLength(50);

            // Indexes
            builder.HasIndex(d => d.PrimaryApplicationId);
            builder.HasIndex(d => d.DuplicateApplicationId);

            // Relationships
            builder.HasOne(d => d.PrimaryApplication)
                .WithMany()
                .HasForeignKey(d => d.PrimaryApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.DuplicateApplication)
                .WithMany()
                .HasForeignKey(d => d.DuplicateApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.ResolvedByUser)
                .WithMany()
                .HasForeignKey(d => d.ResolvedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
