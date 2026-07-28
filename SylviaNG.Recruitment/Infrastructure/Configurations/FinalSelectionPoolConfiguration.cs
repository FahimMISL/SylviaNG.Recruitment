using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class FinalSelectionPoolConfiguration : IEntityTypeConfiguration<FinalSelectionPool>
    {
        public void Configure(EntityTypeBuilder<FinalSelectionPool> builder)
        {
            builder.ToTable("FinalSelectionPools");
            builder.HasKey(p => p.FinalSelectionPoolId);

            builder.Property(p => p.BatchLabel).HasMaxLength(200);

            builder.HasIndex(p => p.OfferLetterId).IsUnique();
            builder.HasIndex(p => p.JobApplicationId);

            builder.HasOne(p => p.OfferLetter)
                .WithMany()
                .HasForeignKey(p => p.OfferLetterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.JobApplication)
                .WithMany()
                .HasForeignKey(p => p.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.PreBoardingSubmission)
                .WithOne(s => s.FinalSelectionPool)
                .HasForeignKey<PreBoardingSubmission>(s => s.FinalSelectionPoolId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
