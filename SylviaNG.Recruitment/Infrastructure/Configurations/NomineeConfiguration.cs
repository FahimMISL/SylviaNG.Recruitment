using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class NomineeConfiguration : IEntityTypeConfiguration<Nominee>
    {
        public void Configure(EntityTypeBuilder<Nominee> builder)
        {
            builder.ToTable("Nominees");
            builder.HasKey(n => n.NomineeId);

            builder.Property(n => n.NomineeName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.Relationship)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(n => n.NationalIdNumber)
                .HasMaxLength(50);

            builder.Property(n => n.Phone)
                .HasMaxLength(50);

            builder.Property(n => n.Address)
                .HasMaxLength(500);

            builder.Property(n => n.SharePercentage)
                .HasColumnType("decimal(5,2)");

            builder.Property(n => n.IdProofFileUrl)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(n => n.PreBoardingProfileId);
        }
    }
}
