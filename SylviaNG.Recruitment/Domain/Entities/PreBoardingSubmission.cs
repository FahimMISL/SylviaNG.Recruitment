using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-12 US-095/096: candidate self-service pre-boarding data collection, 1:1 with a
/// FinalSelectionPool entry. Status starts Draft (auto-created on the candidate's first GET) and
/// locks to Submitted. HR then Validates (-> Approved) or RequestsCorrection (-> NeedsCorrection,
/// which re-opens the form for candidate edits same as Draft) via PreBoardingService. Uses `new`
/// to shadow Audit.Status (int) the same way OfferLetter.Status does.
/// </summary>
public class PreBoardingSubmission : Audit
{
    public long PreBoardingSubmissionId { get; set; }
    public long FinalSelectionPoolId { get; set; }

    public new PreBoardingSubmissionStatusEnum Status { get; set; } = PreBoardingSubmissionStatusEnum.Draft;

    // EP-12 US-096: HR's free-text reason when requesting corrections. Cleared when HR Validates.
    public string? CorrectionComment { get; set; }

    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactRelationship { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;

    public string? InsuranceProvider { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public string? InsuranceNotes { get; set; }

    public string BankName { get; set; } = string.Empty;
    public string? BankBranch { get; set; }
    public string BankAccountName { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public string? BankRoutingNumber { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public FinalSelectionPool FinalSelectionPool { get; set; } = null!;
    public ICollection<PreBoardingNominee> Nominees { get; set; } = new List<PreBoardingNominee>();
}
