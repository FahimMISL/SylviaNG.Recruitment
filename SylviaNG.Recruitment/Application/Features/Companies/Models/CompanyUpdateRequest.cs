namespace SylviaNG.Recruitment.Application.Features.Companies.Models
{
    public class CompanyUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Website { get; set; }
        public string? Industry { get; set; }
        public string? TradeLicenseNumber { get; set; }
        public string? BinNumber { get; set; }
    }
}
