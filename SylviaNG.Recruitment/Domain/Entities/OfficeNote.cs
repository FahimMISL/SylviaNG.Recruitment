using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-12 US-129: a generated office note PDF listing the onboarding enclosures on file for one
/// JobApplication (offer letter, appointment letter, joining booklet - whichever exist at generation
/// time; verification summary is out of scope, EP-11 was dropped entirely from this project).
/// AC3's free-text HR remarks use the inherited Audit.Remarks field directly - a genuine first use
/// of that generic bookkeeping column rather than adding a second, identically-typed property.
/// EnclosuresSummary is a comma-joined snapshot of what existed when generated - same
/// server-rendered/no-preview-edit shape as JoiningBooklet, since there's no per-candidate review UI
/// need here either.
/// </summary>
public class OfficeNote : Audit, ICompanyScoped
{
    public long OfficeNoteId { get; set; }
    public long JobApplicationId { get; set; }
    public long DocumentTemplateId { get; set; }

    // Multi-tenant: denormalized from JobApplication.CompanyId at generation time.
    public long? CompanyId { get; set; }

    public string EnclosuresSummary { get; set; } = string.Empty;
    public string RenderedBody { get; set; } = string.Empty;
    public string GeneratedPdfPath { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }

    public JobApplication JobApplication { get; set; } = null!;
    public DocumentTemplate DocumentTemplate { get; set; } = null!;
}
