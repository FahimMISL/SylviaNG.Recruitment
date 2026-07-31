using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-18 F1: per-tenant corporate branding/theme applied to every generated document (admit
/// card, offer letter, certificates, etc) via the shared Infrastructure/Documents/Shared
/// component layer. Single MISL row seeded for now (Audit.TenantId already scopes it per
/// tenant for later SaaS use - no separate tenant column needed). Company identity/contact
/// fields live here rather than a new Organization entity since none exists yet and one
/// purely for contact fields would be unjustified scope for this feature.
/// </summary>
public class CompanyBranding : Audit
{
    public long CompanyBrandingId { get; set; }

    // Logo - file-storage-backed via IFileStorageService, same shape as JobPostingAttachment;
    // never a DB blob.
    public string? LogoFileName { get; set; }
    public string? LogoStoredFileName { get; set; }
    public string? LogoFilePath { get; set; }
    public string? LogoContentType { get; set; }

    // Company identity/contact - rendered by DocumentFooterComponent.
    public string? CompanyName { get; set; }
    public string? AddressLine { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }

    // Palette / typography
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? AccentColor { get; set; }
    public string? FontFamily { get; set; }

    // Layout
    public HeaderLayoutEnum HeaderLayout { get; set; }
    public FooterLayoutEnum FooterLayout { get; set; }
    public int MarginTop { get; set; }
    public int MarginBottom { get; set; }
    public int MarginLeft { get; set; }
    public int MarginRight { get; set; }
    public DocumentBorderStyleEnum BorderStyle { get; set; }
    public int CornerRadius { get; set; }

    // Watermark
    public bool BackgroundWatermarkEnabled { get; set; }
    public int WatermarkOpacity { get; set; }

    // Dividers
    public DocumentDividerStyleEnum HeaderDividerStyle { get; set; }
    public DocumentDividerStyleEnum FooterDividerStyle { get; set; }

    // Element placement
    public DocumentElementPositionEnum QrCodePosition { get; set; }
    public DocumentElementPositionEnum SignaturePosition { get; set; }
    public DocumentElementPositionEnum SealPosition { get; set; }

    /// <summary>
    /// Token template applied by ReferenceNumberComponent.BuildReferenceNumber: {ORG}, {DOCTYPE},
    /// {YEAR}, {SEQ}. {SEQ} is the calling entity's own DB id today (no dedicated counter table
    /// exists in this codebase yet) - not gap-free sequential numbering.
    /// </summary>
    public string DocumentReferenceFormat { get; set; } = "{ORG}/{DOCTYPE}/{YEAR}/{SEQ}";

    public bool ShowPageNumbers { get; set; }
}
