using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.Companies.Models
{
    public class CompanyResponse
    {
        public long CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Website { get; set; }
        public string? Industry { get; set; }
        public string? TradeLicenseNumber { get; set; }
        public string? BinNumber { get; set; }
        public CompanyStatusEnum Status { get; set; }
        public DateTime? CreatedAt { get; set; }

        public int JobPostingCount { get; set; }
        public int UserAccountCount { get; set; }
    }
}
