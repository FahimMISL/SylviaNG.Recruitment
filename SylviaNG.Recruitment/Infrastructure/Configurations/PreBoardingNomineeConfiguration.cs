using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class PreBoardingNomineeConfiguration : IEntityTypeConfiguration<PreBoardingNominee>
    {
        public void Configure(EntityTypeBuilder<PreBoardingNominee> builder)
        {
            builder.ToTable("PreBoardingNominees");
            builder.HasKey(n => n.PreBoardingNomineeId);

            builder.Property(n => n.FullName).IsRequired().HasMaxLength(200);
            builder.Property(n => n.Relationship).IsRequired().HasMaxLength(100);
            builder.Property(n => n.SharePercentage).HasColumnType("decimal(5,2)");
            builder.Property(n => n.ContactPhone).HasMaxLength(20);
            builder.Property(n => n.Address).HasMaxLength(500);

            builder.HasIndex(n => n.PreBoardingSubmissionId);

            builder.HasOne(n => n.PreBoardingSubmission)
                .WithMany(s => s.Nominees)
                .HasForeignKey(n => n.PreBoardingSubmissionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
