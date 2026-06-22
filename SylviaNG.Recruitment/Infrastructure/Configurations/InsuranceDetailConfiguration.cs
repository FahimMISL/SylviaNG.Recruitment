using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class InsuranceDetailConfiguration : IEntityTypeConfiguration<InsuranceDetail>
    {
        public void Configure(EntityTypeBuilder<InsuranceDetail> builder)
        {
            builder.ToTable("InsuranceDetails");
            builder.HasKey(i => i.InsuranceDetailId);

            builder.Property(i => i.InsuranceType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.ProviderName)
                .HasMaxLength(200);

            builder.Property(i => i.PolicyNumber)
                .HasMaxLength(100);

            builder.Property(i => i.BeneficiaryName)
                .HasMaxLength(200);

            builder.Property(i => i.BeneficiaryRelationship)
                .HasMaxLength(100);

            builder.Property(i => i.DocumentFileUrl)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(i => i.PreBoardingProfileId);
        }
    }
}
