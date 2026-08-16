using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CompanyBranding.Models
{
    // Company identity/contact (name/address/phone/email/website) is no longer editable here -
    // it's sourced live from Company (edited via Company Management) so the two never drift.
    public class CompanyBrandingUpdateRequest
    {
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
