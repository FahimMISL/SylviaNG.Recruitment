using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-10 US-081: a generated offer letter for one JobApplication. Designation/OfferedSalary/
/// JoiningDate/ReportingManager/OfferValidityDate are HR-entered at generation time - no EP-12
/// fitment-data entity exists yet to source them from (EP-12 is scheduled after EP-10), so this
/// request payload is the fitment-data hook for now. ReportingManager is free text, not an
/// Employee FK - no Employee-picker widget precedent exists (deferred in US-062).
/// </summary>
public class OfferLetter : Audit, ICompanyScoped
{
    public long OfferLetterId { get; set; }
    public long JobApplicationId { get; set; }
    public long DocumentTemplateId { get; set; }

    // Multi-tenant: denormalized from JobApplication.CompanyId at generation time.
    public long? CompanyId { get; set; }

    public string Designation { get; set; } = string.Empty;
    public decimal OfferedSalary { get; set; }
    public DateTime JoiningDate { get; set; }
    public string? ReportingManager { get; set; }
    public DateTime? OfferValidityDate { get; set; }

    public string GeneratedPdfPath { get; set; } = string.Empty;
    public new OfferLetterStatusEnum Status { get; set; } = OfferLetterStatusEnum.Generated;
    public DateTime GeneratedAt { get; set; }

    // EP-10 US-082: set together when the candidate accepts/declines. DecisionAt being non-null
    // is the lock check AcceptAsync/DeclineAsync use to reject a second decision.
    public DateTime? DecisionAt { get; set; }
    public string? DeclineReason { get; set; }

    public JobApplication JobApplication { get; set; } = null!;
    public DocumentTemplate DocumentTemplate { get; set; } = null!;
    public ICollection<AppointmentLetter> AppointmentLetters { get; set; } = new List<AppointmentLetter>();
    public ICollection<JoiningBooklet> JoiningBooklets { get; set; } = new List<JoiningBooklet>();
    public ICollection<MedicalLetter> MedicalLetters { get; set; } = new List<MedicalLetter>();
    public ICollection<TargetLetter> TargetLetters { get; set; } = new List<TargetLetter>();
}
