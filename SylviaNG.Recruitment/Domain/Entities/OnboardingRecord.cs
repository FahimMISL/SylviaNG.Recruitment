using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class OnboardingRecord : Audit
{
    public long OnboardingRecordId { get; set; }
    public long CandidateId { get; set; }
    public long JobApplicationId { get; set; }
    public OnboardingStageEnum Stage { get; set; } = OnboardingStageEnum.PreHire;
    public string? CoreHrEmployeeId { get; set; }
    public string? PayrollReferenceId { get; set; }
    public DateTime? PreHireSentAt { get; set; }
    public DateTime? PostHireSentAt { get; set; }
    public bool IsPreHireSuccess { get; set; }
    public bool IsPostHireSuccess { get; set; }
    public string? FailureDetails { get; set; }
    public int RetryCount { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Candidate Candidate { get; set; } = null!;
    public JobApplication JobApplication { get; set; } = null!;
}
