using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class OfferCompensationConfiguration : IEntityTypeConfiguration<OfferCompensation>
    {
        public void Configure(EntityTypeBuilder<OfferCompensation> builder)
        {
            builder.ToTable("OfferCompensations");
            builder.HasKey(o => o.OfferCompensationId);

            builder.Property(o => o.ComponentName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.Currency)
                .HasMaxLength(10);

            builder.Property(o => o.Frequency)
                .HasMaxLength(50);

            // Indexes
            builder.HasIndex(o => o.JobApplicationId);
            builder.HasIndex(o => o.FitmentDataId);

            // Relationships
            builder.HasOne(o => o.JobApplication)
                .WithMany()
                .HasForeignKey(o => o.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.FitmentData)
                .WithMany()
                .HasForeignKey(o => o.FitmentDataId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
