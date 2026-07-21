using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class UniversityLibraryItemConfiguration : IEntityTypeConfiguration<UniversityLibraryItem>
    {
        private static readonly DateTime SeedCreatedAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Best-effort list of well-known Bangladeshi public/private universities (full name +
        // short code) - not exhaustive. A candidate whose institution isn't listed here still
        // just types it as free text (see UniversityLibraryItemId being nullable on CandidateEducation).
        private static readonly (string Name, string Code)[] Universities =
        {
            ("University of Dhaka", "DU"),
            ("University of Rajshahi", "RU"),
            ("University of Chittagong", "CU"),
            ("Jahangirnagar University", "JU"),
            ("Bangladesh University of Engineering and Technology", "BUET"),
            ("Khulna University", "KU"),
            ("Shahjalal University of Science and Technology", "SUST"),
            ("Islamic University, Bangladesh", "IU"),
            ("Jagannath University", "JnU"),
            ("Comilla University", "CoU"),
            ("Jatiya Kabi Kazi Nazrul Islam University", "JKKNIU"),
            ("Begum Rokeya University, Rangpur", "BRUR"),
            ("Noakhali Science and Technology University", "NSTU"),
            ("Patuakhali Science and Technology University", "PSTU"),
            ("Chittagong University of Engineering & Technology", "CUET"),
            ("Rajshahi University of Engineering & Technology", "RUET"),
            ("Khulna University of Engineering & Technology", "KUET"),
            ("Dhaka University of Engineering & Technology, Gazipur", "DUET"),
            ("Bangladesh Agricultural University", "BAU"),
            ("Sher-e-Bangla Agricultural University", "SAU"),
            ("Sylhet Agricultural University", "SYLAU"),
            ("Bangabandhu Sheikh Mujibur Rahman Agricultural University", "BSMRAU"),
            ("Hajee Mohammad Danesh Science and Technology University", "HSTU"),
            ("Pabna University of Science and Technology", "PUST"),
            ("Jessore University of Science and Technology", "JUST"),
            ("Bangabandhu Sheikh Mujib Medical University", "BSMMU"),
            ("National University, Bangladesh", "NU"),
            ("Bangladesh Open University", "BOU"),
            ("Bangladesh University of Professionals", "BUP"),
            ("Mawlana Bhashani Science and Technology University", "MBSTU"),
            ("Bangladesh University of Textiles", "BUTEX"),
            ("North South University", "NSU"),
            ("BRAC University", "BRACU"),
            ("Independent University, Bangladesh", "IUB"),
            ("American International University-Bangladesh", "AIUB"),
            ("Ahsanullah University of Science and Technology", "AUST"),
            ("Islamic University of Technology", "IUT"),
            ("United International University", "UIU"),
            ("East West University", "EWU"),
            ("University of Liberal Arts Bangladesh", "ULAB"),
            ("Daffodil International University", "DIU"),
            ("Southeast University", "SEU"),
            ("Green University of Bangladesh", "GUB"),
            ("Stamford University Bangladesh", "SUB"),
            ("World University of Bangladesh", "WUB"),
            ("Presidency University", "PU"),
            ("International Islamic University Chittagong", "IIUC"),
            ("International University of Business Agriculture and Technology", "IUBAT"),
            ("Prime University", "PU2"),
            ("City University", "CityU"),
            ("Northern University Bangladesh", "NUB"),
            ("Manarat International University", "MIU"),
            ("Uttara University", "UU"),
            ("University of Asia Pacific", "UAP"),
            ("Asian University of Bangladesh", "AUB"),
            ("Eastern University", "EU"),
            ("Premier University, Chattogram", "PUC"),
            ("Leading University, Sylhet", "LUS"),
            ("Metropolitan University, Sylhet", "MU"),
            ("Shanto-Mariam University of Creative Technology", "SMUCT"),
            ("ASA University Bangladesh", "ASAUB"),
            ("Victoria University of Bangladesh", "VUB"),
            ("Southern University Bangladesh", "SUBD"),
            ("University of Development Alternative", "UODA"),
            ("Central Women's University", "CWU"),
            ("Feni University", "FU"),
            ("Britannia University", "BRIT"),
            ("Notre Dame University Bangladesh", "NDUB"),
            ("Varendra University", "VU2"),
            ("Bangladesh Army University of Science and Technology", "BAUST"),
            ("Military Institute of Science and Technology", "MIST"),
            ("Bangladesh University of Business and Technology", "BUBT"),
            ("Canadian University of Bangladesh", "CUB"),
            ("European University of Bangladesh", "EUB"),
            ("Gono Bishwabidyalay", "GB"),
            ("North Western University", "NWU"),
            ("Sonargaon University", "SU2"),
            ("University of Information Technology and Sciences", "UITS"),
            ("Z. H. Sikder University of Science & Technology", "ZHSUST"),
        };

        public void Configure(EntityTypeBuilder<UniversityLibraryItem> builder)
        {
            builder.ToTable("UniversityLibraryItems");
            builder.HasKey(u => u.UniversityLibraryItemId);

            builder.Property(u => u.Name).IsRequired().HasMaxLength(200);
            builder.Property(u => u.Code).IsRequired().HasMaxLength(20);
            builder.HasIndex(u => u.Name).IsUnique();

            builder.HasData(Universities.Select((u, index) => new
            {
                UniversityLibraryItemId = (long)(index + 1),
                u.Name,
                u.Code,
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
