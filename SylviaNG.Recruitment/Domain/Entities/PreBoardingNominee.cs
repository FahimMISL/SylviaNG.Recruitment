using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>EP-12 US-095: a beneficiary nominee on a PreBoardingSubmission. Saved as a full-replace
/// (delete+reinsert) of the parent's Nominees collection on every SaveDraft/Submit, not individually
/// CRUD'd - see PreBoardingService.SaveDraftAsync.</summary>
public class PreBoardingNominee : Audit
{
    public long PreBoardingNomineeId { get; set; }
    public long PreBoardingSubmissionId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public decimal SharePercentage { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }

    public PreBoardingSubmission PreBoardingSubmission { get; set; } = null!;
}
