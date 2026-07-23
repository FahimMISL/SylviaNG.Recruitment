using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class DegreeConfiguration : IEntityTypeConfiguration<Degree>
    {
        private static readonly DateTime SeedCreatedAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Matches the company's reference Millennium HR Degree List exactly: Position groups
        // academically-equivalent degrees. 1 = below/at SSC level, 2 = at HSC level (Board
        // dropdown applies to 1 and 2), 3 = Bachelor-level, 4 = Master-level.
        private static readonly (string Name, string FullName, int Position)[] Degrees =
        {
            ("Below SSC", "Below SSC", 1),
            ("Dakhil", "Dakhil", 1),
            ("O Level", "Ordinary Level", 1),
            ("SSC", "Secondary School Certificate", 1),

            ("A Level", "Advance Level", 2),
            ("Alim", "Alim", 2),
            ("Diploma (Comm)", "Diploma in Commerce", 2),
            ("Diploma (Engg)", "Diploma-in-Engineering", 2),
            ("GED", "The General Education", 2),
            ("HSC", "Higher Secondary School Certificate", 2),

            ("B.Sc. Agril. Engg.", "Bachelor of Science in Agricultural Engineering", 3),
            ("B.Sc. Food Engg.", "Bachelor of Science in Food Engineering", 3),
            ("BA (Hons)", "Bachelor of Arts (Honours)", 3),
            ("BA (Pass)", "Bachelor of Arts (Pass)", 3),
            ("BArch", "Bachelor of Architecture", 3),
            ("BBA", "Bachelor of Business Administration", 3),
            ("BBA (Hons)", "Bachelor of Business Administration", 3),
            ("BBM", "Bachelor of Bank Management", 3),
            ("BBS (Hons)", "Bachelor of Business Studies (Honours)", 3),
            ("BBS (Pass)", "Bachelor of Business Studies (Pass)", 3),
            ("BCA (Hons)", "Bachelor of Computer Application", 3),
            ("BCom (Hons)", "Bachelor of Commerce (Honours)", 3),
            ("BCom (Pass)", "Bachelor of Commerce (Pass)", 3),
            ("BEd", "Bachelor of Education", 3),
            ("BSS (Hons)", "Bachelor of Social Science (Honours)", 3),
            ("BSS (Pass)", "Bachelor of Social Science (Pass)", 3),
            ("BSc (Engg)", "Bachelor of Science (Engineering)", 3),
            ("BSc (Hons)", "Bachelor of Science (Honours)", 3),
            ("BSc (Pass)", "Bachelor of Science (Pass)", 3),
            ("BTech", "Bachelor of Technology", 3),
            ("Fazil", "Fazil", 3),
            ("LLB", "Bachelor of Law", 3),
            ("MBBS", "Bachelor of Medicine, Bachelor of Surgery", 3),

            ("Kamil", "Kamil", 4),
            ("LLM", "Master of Law", 4),
            ("MA", "Master of Arts", 4),
            ("MBA", "Master of Business Administration", 4),
            ("MBM", "Master of Banking Management", 4),
            ("MBS", "Master of Business Studies", 4),
            ("MCom", "Master of Commerce", 4),
            ("MEd", "Master of Education", 4),
            ("MIT", "Master in Information Technology", 4),
            ("MS", "Master of Science", 4),
        };

        public void Configure(EntityTypeBuilder<Degree> builder)
        {
            builder.ToTable("Degrees");
            builder.HasKey(d => d.DegreeId);

            builder.Property(d => d.Name).IsRequired().HasMaxLength(50);
            builder.Property(d => d.FullName).IsRequired().HasMaxLength(200);
            builder.Property(d => d.Position).IsRequired();
            builder.HasIndex(d => d.Name).IsUnique();

            builder.HasData(Degrees.Select((d, index) => new
            {
                DegreeId = (long)(index + 1),
                d.Name,
                d.FullName,
                d.Position,
                TenantId = "default_tenant",
                Remarks = (string?)null,
                CreatedAt = SeedCreatedAt,
                CreatedBy = 1L,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (long?)null,
                DeletedAt = (DateTime?)null,
                DeletedBy = (long?)null,
                Status = 1
            }));
        }
    }
}
