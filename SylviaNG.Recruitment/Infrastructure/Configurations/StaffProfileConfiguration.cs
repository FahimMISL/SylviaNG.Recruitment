using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class StaffProfileConfiguration : IEntityTypeConfiguration<StaffProfile>
    {
        public void Configure(EntityTypeBuilder<StaffProfile> builder)
        {
            builder.ToTable("StaffProfiles");
            builder.HasKey(s => s.StaffProfileId);

            builder.Property(s => s.KeycloakSubjectId).IsRequired().HasMaxLength(100);
            builder.Property(s => s.FullName).IsRequired().HasMaxLength(200);
            builder.Property(s => s.ProfilePhotoPath).HasMaxLength(500);

            builder.HasIndex(s => s.KeycloakSubjectId).IsUnique();
        }
    }
}
