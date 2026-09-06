using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-10 US-083: a generated appointment letter for one Accepted OfferLetter. Designation/salary/
/// joining-date are not duplicated here - they're already on the linked OfferLetter. FinalBody is
/// the placeholder-substituted text HR actually reviewed/adjusted (via DocumentTemplateController's
/// existing Preview endpoint) and submitted at generation time - persisting it is the AC5 document
/// history record, no separate history entity needed.
/// </summary>
public class AppointmentLetter : Audit, ICompanyScoped
{
    public long AppointmentLetterId { get; set; }
    public long JobApplicationId { get; set; }
    public long OfferLetterId { get; set; }
    public long DocumentTemplateId { get; set; }

    // Multi-tenant: denormalized from JobApplication.CompanyId at generation time.
    public long? CompanyId { get; set; }

    public string FinalBody { get; set; } = string.Empty;
    public string GeneratedPdfPath { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }

    public JobApplication JobApplication { get; set; } = null!;
    public OfferLetter OfferLetter { get; set; } = null!;
    public DocumentTemplate DocumentTemplate { get; set; } = null!;
}
