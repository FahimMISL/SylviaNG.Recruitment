using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-10 US-084: a generated joining booklet for one Accepted OfferLetter, produced individually
/// or as part of a bulk-generate batch. BatchLabel/JoiningDate are HR-entered per batch - no
/// FinalSelectionPool entity exists yet (that's US-094/EP-12, scheduled after EP-10), so this is
/// the lightweight batch stub: HR groups Accepted-offer candidates ad hoc instead of picking a
/// persisted pool. RenderedBody is server-rendered (no client preview-edit step, unlike
/// AppointmentLetter/MedicalLetter/TargetLetter) since bulk generation has no per-candidate review
/// UI - it's regenerated from this same body for the bulk-download ZIP rather than re-reading a
/// saved PDF from disk.
/// </summary>
public class JoiningBooklet : Audit, ICompanyScoped
{
    public long JoiningBookletId { get; set; }
    public long JobApplicationId { get; set; }
    public long OfferLetterId { get; set; }
    public long DocumentTemplateId { get; set; }

    // Multi-tenant: denormalized from JobApplication.CompanyId at generation time.
    public long? CompanyId { get; set; }

    public string BatchLabel { get; set; } = string.Empty;
    public DateTime JoiningDate { get; set; }
    public string RenderedBody { get; set; } = string.Empty;
    public string GeneratedPdfPath { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }

    public JobApplication JobApplication { get; set; } = null!;
    public OfferLetter OfferLetter { get; set; } = null!;
    public DocumentTemplate DocumentTemplate { get; set; } = null!;
}
