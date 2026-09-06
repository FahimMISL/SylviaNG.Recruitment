using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class FitmentDataConfiguration : IEntityTypeConfiguration<FitmentData>
    {
        public void Configure(EntityTypeBuilder<FitmentData> builder)
        {
            builder.ToTable("FitmentDatas");
            builder.HasKey(f => f.FitmentDataId);

            builder.Property(f => f.Designation).IsRequired().HasMaxLength(200);
            builder.Property(f => f.Grade).HasMaxLength(100);
            builder.Property(f => f.Location).HasMaxLength(200);

            builder.HasIndex(f => f.JobApplicationId).IsUnique();
            builder.HasIndex(f => f.CompanyId);

            builder.HasOne(f => f.JobApplication)
                .WithOne(a => a.FitmentData)
                .HasForeignKey<FitmentData>(f => f.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
