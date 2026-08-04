using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-10 US-086: a generated medical referral letter for one Accepted OfferLetter. MedicalTestCenter/
/// RequiredTests are HR-entered at generation time (no test-catalog entity exists yet), same
/// fitment-data-hook pattern as OfferLetter.Designation/ReportingManager. FinalBody is the
/// placeholder-substituted text HR reviewed/adjusted via DocumentTemplateController's Preview
/// endpoint and submitted at generation time - persisting it is the AC4 document history record.
/// </summary>
public class MedicalLetter : Audit
{
    public long MedicalLetterId { get; set; }
    public long JobApplicationId { get; set; }
    public long OfferLetterId { get; set; }
    public long DocumentTemplateId { get; set; }

    public string MedicalTestCenter { get; set; } = string.Empty;
    public string RequiredTests { get; set; } = string.Empty;
    public string FinalBody { get; set; } = string.Empty;
    public string GeneratedPdfPath { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }

    public JobApplication JobApplication { get; set; } = null!;
    public OfferLetter OfferLetter { get; set; } = null!;
    public DocumentTemplate DocumentTemplate { get; set; } = null!;
}
