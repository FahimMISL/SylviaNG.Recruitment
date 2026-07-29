using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CompanyBranding.Models
{
    public class CompanyBrandingUpdateRequest
    {
        public string? CompanyName { get; set; }
        public string? AddressLine { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? AccentColor { get; set; }
        public string? FontFamily { get; set; }
        public DocumentBorderStyleEnum BorderStyle { get; set; }
        public bool BackgroundWatermarkEnabled { get; set; }
        public int WatermarkOpacity { get; set; }
        public bool ShowPageNumbers { get; set; }
    }
}
