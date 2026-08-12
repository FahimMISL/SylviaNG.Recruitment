using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Represents a candidate's application to a job posting.
/// </summary>
public class JobApplication : Audit
{
    public long JobApplicationId { get; set; }
    public long JobPostingId { get; set; }

    // Resolved at submission time when a matching CandidateProfile exists (authenticated
    // submitter, or an existing profile matching CandidateEmail); null for a guest applicant
    // with no profile yet. Backfilled once they later register (see
    // CurrentCandidateService.GetOrCreateCurrentProfileAsync's claim step). CandidateName/
    // Email/Phone/NationalId stay as the point-in-time snapshot the applicant typed.
    public long? CandidateProfileId { get; set; }

    public string CandidateName { get; set; } = string.Empty;
    public string? CandidateEmail { get; set; }
    public string? CandidatePhone { get; set; }
    public string? CandidateNationalId { get; set; }
    public string? ResumeUrl { get; set; }
    public string? CoverLetter { get; set; }

    // Raw text extracted from the resume file at submission time, so CV Bank search (US-045)
    // can match against CV content without re-extracting the file on every search.
    public string? ResumeExtractedText { get; set; }
    public ApplicationStatusEnum ApplicationStatus { get; set; } = ApplicationStatusEnum.Applied;
    public DateTime? AppliedDate { get; set; }
    public bool IsActive { get; set; } = true;
    public ApplicationSourceEnum Source { get; set; } = ApplicationSourceEnum.External;

    // EP-17/US-127: optionally declared at apply time, used for fee-waiver rule matching and F1
    // reconciliation reporting. WaiverRuleId/WaivedAt are stamped by the system when a WaiverRule
    // matches at submission - see JobApplicationService.SubmitAsync.
    public long? SpecialCategoryId { get; set; }
    public long? ReferralSourceId { get; set; }
    public long? WaiverRuleId { get; set; }
    public DateTime? WaivedAt { get; set; }

    // Proof document for the claimed SpecialCategoryId (e.g. freedom-fighter certificate) -
    // required for TryMatchAsync to actually waive the fee; without it the category is recorded
    // but the applicant still pays. See JobApplicationService.SubmitAsync.
    public string? WaiverProofDocumentUrl { get; set; }

    // Navigation properties
    public JobPosting JobPosting { get; set; } = null!;
    public CandidateProfile? CandidateProfile { get; set; }
    public SpecialCategory? SpecialCategory { get; set; }
    public ReferralSource? ReferralSource { get; set; }
    public WaiverRule? WaiverRule { get; set; }
    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
    public ICollection<ApplicationStatusHistory> StatusHistory { get; set; } = new List<ApplicationStatusHistory>();
    public ICollection<JobApplicationStageProgress> StageProgress { get; set; } = new List<JobApplicationStageProgress>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<OfferLetter> OfferLetters { get; set; } = new List<OfferLetter>();
    public ICollection<AppointmentLetter> AppointmentLetters { get; set; } = new List<AppointmentLetter>();
    public ICollection<JoiningBooklet> JoiningBooklets { get; set; } = new List<JoiningBooklet>();
    public ICollection<MedicalLetter> MedicalLetters { get; set; } = new List<MedicalLetter>();
    public ICollection<TargetLetter> TargetLetters { get; set; } = new List<TargetLetter>();
    public FitmentData? FitmentData { get; set; }
    public ICollection<OfficeNote> OfficeNotes { get; set; } = new List<OfficeNote>();
}
