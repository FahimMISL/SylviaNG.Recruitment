using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ProfileFieldConfigConfiguration : IEntityTypeConfiguration<ProfileFieldConfig>
    {
        public void Configure(EntityTypeBuilder<ProfileFieldConfig> builder)
        {
            builder.ToTable("ProfileFieldConfigs");
            builder.HasKey(c => c.ProfileFieldConfigId);

            builder.HasOne(c => c.JobPosting)
                .WithMany()
                .HasForeignKey(c => c.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
