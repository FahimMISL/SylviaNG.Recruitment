using System.ComponentModel.DataAnnotations.Schema;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-18 F1: per-company corporate branding/theme applied to every generated document (admit
/// card, offer letter, certificates, etc) via the shared Infrastructure/Documents/Shared
/// component layer. Company-scoped like the rest of the multi-tenancy model (see
/// ICompanyScoped) - one row lazily created per Company on first save.
/// </summary>
public class CompanyBranding : Audit, ICompanyScoped
{
    public long CompanyBrandingId { get; set; }

    // Multi-tenant: which Company this branding belongs to.
    public long? CompanyId { get; set; }
    public Company? Company { get; set; }

    // Logo - file-storage-backed via IFileStorageService, same shape as JobPostingAttachment;
    // never a DB blob.
    public string? LogoFileName { get; set; }
    public string? LogoStoredFileName { get; set; }
    public string? LogoFilePath { get; set; }
    public string? LogoContentType { get; set; }

    // Company identity/contact - rendered by DocumentFooterComponent. Not a persisted column:
    // BrandingResolverService populates these in-memory from the linked Company (single source
    // of truth is Company.Name/Email/Phone/Address/Website, edited via Company Management) so
    // every document generator keeps reading them off this entity unchanged.
    [NotMapped]
    public string? CompanyName { get; set; }
    [NotMapped]
    public string? AddressLine { get; set; }
    [NotMapped]
    public string? Phone { get; set; }
    [NotMapped]
    public string? Email { get; set; }
    [NotMapped]
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
