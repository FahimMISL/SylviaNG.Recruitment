using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations;

public class UserProfilePhotoConfiguration : IEntityTypeConfiguration<UserProfilePhoto>
{
    public void Configure(EntityTypeBuilder<UserProfilePhoto> builder)
    {
        builder.ToTable("UserProfilePhotos");
        builder.HasKey(p => p.UserProfilePhotoId);

        builder.Property(p => p.KeycloakUserId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.FileName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.ContentType)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.KeycloakUserId).IsUnique();
    }
}
