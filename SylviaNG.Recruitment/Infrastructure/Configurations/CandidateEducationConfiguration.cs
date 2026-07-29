using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class CandidateEducationConfiguration : IEntityTypeConfiguration<CandidateEducation>
    {
        public void Configure(EntityTypeBuilder<CandidateEducation> builder)
        {
            builder.ToTable("CandidateEducations");
            builder.HasKey(e => e.CandidateEducationId);

            builder.Property(e => e.Institution).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Result).IsRequired().HasMaxLength(50);
            builder.Property(e => e.MajorSubjectOtherText).HasMaxLength(200);
            builder.Ignore(e => e.MajorSubjectDisplay);

            builder.Property(e => e.EducationLevel)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(e => e.GradingSystem)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasIndex(e => e.CandidateProfileId);

            builder.HasOne(e => e.CandidateProfile)
                .WithMany(c => c.Educations)
                .HasForeignKey(e => e.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.UniversityLibraryItem)
                .WithMany()
                .HasForeignKey(e => e.UniversityLibraryItemId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(e => e.Degree)
                .WithMany()
                .HasForeignKey(e => e.DegreeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.EducationBoard)
                .WithMany()
                .HasForeignKey(e => e.EducationBoardId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(e => e.MajorSubjectSscHsc)
                .WithMany()
                .HasForeignKey(e => e.MajorSubjectSscHscId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(e => e.MajorSubjectUniversity)
                .WithMany()
                .HasForeignKey(e => e.MajorSubjectUniversityId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
