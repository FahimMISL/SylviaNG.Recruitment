using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CompanyBranding.Models
{
    /// <summary>
    /// EP-18 F3: the trimmed field set exposed by this round's minimal branding-settings screen.
    /// Margins, corner radius, header/footer layout variants, dividers, QR/signature/seal
    /// positions, and the reference-number format stay at their DB defaults (not editable here) -
    /// exposed in a later, fuller admin-editor round.
    /// </summary>
    public class CompanyBrandingResponse
    {
        public string? LogoFilePath { get; set; }
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
