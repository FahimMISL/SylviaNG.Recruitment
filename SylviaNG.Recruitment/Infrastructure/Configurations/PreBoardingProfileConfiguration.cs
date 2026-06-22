using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class PreBoardingProfileConfiguration : IEntityTypeConfiguration<PreBoardingProfile>
    {
        public void Configure(EntityTypeBuilder<PreBoardingProfile> builder)
        {
            builder.ToTable("PreBoardingProfiles");
            builder.HasKey(p => p.PreBoardingProfileId);

            builder.Property(p => p.ProfileStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(p => p.CorrectionComments)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(p => p.CandidateId);
            builder.HasIndex(p => p.JobApplicationId);
            builder.HasIndex(p => p.ProfileStatus);

            // Relationships
            builder.HasOne(p => p.Candidate)
                .WithMany()
                .HasForeignKey(p => p.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.JobApplication)
                .WithMany()
                .HasForeignKey(p => p.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.ValidatedByUser)
                .WithMany()
                .HasForeignKey(p => p.ValidatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Nominees)
                .WithOne(n => n.PreBoardingProfile)
                .HasForeignKey(n => n.PreBoardingProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.EmergencyContacts)
                .WithOne(e => e.PreBoardingProfile)
                .HasForeignKey(e => e.PreBoardingProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.InsuranceDetails)
                .WithOne(i => i.PreBoardingProfile)
                .HasForeignKey(i => i.PreBoardingProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
