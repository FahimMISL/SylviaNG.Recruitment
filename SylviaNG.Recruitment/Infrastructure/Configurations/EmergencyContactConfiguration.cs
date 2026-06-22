using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class EmergencyContactConfiguration : IEntityTypeConfiguration<EmergencyContact>
    {
        public void Configure(EntityTypeBuilder<EmergencyContact> builder)
        {
            builder.ToTable("EmergencyContacts");
            builder.HasKey(e => e.EmergencyContactId);

            builder.Property(e => e.ContactName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Relationship)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Phone)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.AlternatePhone)
                .HasMaxLength(50);

            builder.Property(e => e.Address)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(e => e.PreBoardingProfileId);
        }
    }
}
