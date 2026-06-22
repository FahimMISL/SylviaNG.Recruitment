using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class RecruitmentAgencyConfiguration : IEntityTypeConfiguration<RecruitmentAgency>
    {
        public void Configure(EntityTypeBuilder<RecruitmentAgency> builder)
        {
            builder.ToTable("RecruitmentAgencies");
            builder.HasKey(a => a.RecruitmentAgencyId);

            builder.Property(a => a.AgencyName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.ContactPerson)
                .HasMaxLength(200);

            builder.Property(a => a.Email)
                .HasMaxLength(200);

            builder.Property(a => a.Phone)
                .HasMaxLength(50);

            builder.Property(a => a.AgreementDetails)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(a => a.AgencyName);

            // Relationships
            builder.HasMany(a => a.Referrals)
                .WithOne(r => r.RecruitmentAgency)
                .HasForeignKey(r => r.RecruitmentAgencyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
