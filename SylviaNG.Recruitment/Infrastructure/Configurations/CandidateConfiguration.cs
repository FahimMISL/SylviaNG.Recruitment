using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {
            builder.ToTable("Candidates");
            builder.HasKey(c => c.CandidateId);

            builder.Property(c => c.KeycloakUserId)
                .HasMaxLength(200);

            builder.Property(c => c.CandidateType)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(c => c.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Email)
                .HasMaxLength(200);

            builder.Property(c => c.Phone)
                .HasMaxLength(50);

            builder.Property(c => c.NationalIdNumber)
                .HasMaxLength(50);

            builder.Property(c => c.Gender)
                .HasMaxLength(20);

            builder.Property(c => c.Address)
                .HasMaxLength(500);

            builder.Property(c => c.City)
                .HasMaxLength(100);

            builder.Property(c => c.Country)
                .HasMaxLength(100);

            builder.Property(c => c.ProfilePhotoUrl)
                .HasMaxLength(500);

            builder.Property(c => c.CurrentDesignation)
                .HasMaxLength(200);

            builder.Property(c => c.CurrentOrganization)
                .HasMaxLength(200);

            builder.Property(c => c.ExpectedSalary)
                .HasMaxLength(100);

            builder.Property(c => c.PresentAddress).HasMaxLength(500);
            builder.Property(c => c.PermanentAddress).HasMaxLength(500);
            builder.Property(c => c.LinkedInUrl).HasMaxLength(300);
            builder.Property(c => c.GitHubUrl).HasMaxLength(300);
            builder.Property(c => c.PortfolioUrl).HasMaxLength(300);
            builder.Property(c => c.FatherName).HasMaxLength(200);
            builder.Property(c => c.MotherName).HasMaxLength(200);
            builder.Property(c => c.MaritalStatus).HasMaxLength(30);
            builder.Property(c => c.Religion).HasMaxLength(50);
            builder.Property(c => c.BloodGroup).HasMaxLength(10);

            // Indexes
            builder.HasIndex(c => c.Email);
            builder.HasIndex(c => c.NationalIdNumber);
            builder.HasIndex(c => c.KeycloakUserId);
            builder.HasIndex(c => c.EmployeeId);

            // Relationships
            builder.HasOne(c => c.Employee)
                .WithMany()
                .HasForeignKey(c => c.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Educations)
                .WithOne(e => e.Candidate)
                .HasForeignKey(e => e.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Experiences)
                .WithOne(e => e.Candidate)
                .HasForeignKey(e => e.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Skills)
                .WithOne(s => s.Candidate)
                .HasForeignKey(s => s.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Certifications)
                .WithOne(cert => cert.Candidate)
                .HasForeignKey(cert => cert.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Documents)
                .WithOne(d => d.Candidate)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Applications)
                .WithOne(a => a.Candidate)
                .HasForeignKey(a => a.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
