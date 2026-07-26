using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class PreBoardingSubmissionConfiguration : IEntityTypeConfiguration<PreBoardingSubmission>
    {
        public void Configure(EntityTypeBuilder<PreBoardingSubmission> builder)
        {
            builder.ToTable("PreBoardingSubmissions");
            builder.HasKey(s => s.PreBoardingSubmissionId);

            builder.Property(s => s.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(s => s.EmergencyContactName).IsRequired().HasMaxLength(200);
            builder.Property(s => s.EmergencyContactRelationship).IsRequired().HasMaxLength(100);
            builder.Property(s => s.EmergencyContactPhone).IsRequired().HasMaxLength(20);

            builder.Property(s => s.InsuranceProvider).HasMaxLength(200);
            builder.Property(s => s.InsurancePolicyNumber).HasMaxLength(100);
            builder.Property(s => s.InsuranceNotes).HasMaxLength(1000);

            builder.Property(s => s.BankName).IsRequired().HasMaxLength(200);
            builder.Property(s => s.BankBranch).HasMaxLength(200);
            builder.Property(s => s.BankAccountName).IsRequired().HasMaxLength(200);
            builder.Property(s => s.BankAccountNumber).IsRequired().HasMaxLength(50);
            builder.Property(s => s.BankRoutingNumber).HasMaxLength(50);

            builder.HasIndex(s => s.FinalSelectionPoolId).IsUnique();
        }
    }
}
